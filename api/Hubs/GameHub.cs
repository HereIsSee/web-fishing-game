using Api.Models;
using Api.Models.Facade;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class GameHub : Hub
    {
        private static Session _session = Session.Instance;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly ILogger<GameHub> _logger;
        private readonly GameFacade _gameFacade = new GameFacade();

        public GameHub(IHubContext<GameHub> hubContext, ILogger<GameHub> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task JoinSession(string playerName)
        {
            Console.WriteLine($"🎣 Player {playerName} joining session...");

            // When a player joins the game create a new boat for him
            _session.AddPlayer(Context.ConnectionId, playerName);

            var player = _session.GetPlayer(Context.ConnectionId);

            var allPlayers = _session.GetAllPlayers();
            await Clients.Caller.SendAsync("ReceiveAllPlayers", allPlayers);

            await Clients.All.SendAsync("PlayerJoined", player);
            await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);

            _gameFacade.RenderAllPlayers(allPlayers);

            Console.WriteLine($"✅ Player {playerName} joined!");
            Console.WriteLine($"Player {Context.ConnectionId} connection id!");
            Console.WriteLine($"📊 Sent {allPlayers.Count} existing players to new player");

            Console.WriteLine(_session.IsActive);
            if (!_session.IsActive)
            {
                Console.WriteLine("First player joined — starting game automatically!");
                await StartGame();
            }
            await SendScoreboardUpdate();
        }
        public async Task LeaveSession()
        {
            _session.RemovePlayer(Context.ConnectionId);
            await Clients.All.SendAsync("PlayerLeft", Context.ConnectionId);
            await SendScoreboardUpdate();
        }

        public async Task StartGame()
        {
            _session.StartGame();
            _gameFacade.InitializeGame();
            await Clients.All.SendAsync("GameStarted", _session.TimerDuration);
            _ = Task.Run(async () =>
            {
                while (_session.IsActive)
                {
                    _session.Environment.Update();

                    await _hubContext.Clients.All.SendAsync("UpdateFishes", _session.Environment.Fishes);
                    
                    if (_session.State == GameState.Playing)
                    {
                        var elapsedSeconds = (int)(DateTime.UtcNow - _session.StartTime).TotalSeconds;
                        if (elapsedSeconds >= _session.TimerDuration)
                        {
                            Console.WriteLine("⏰ Game time expired! Ending game...");
                            _session.EndGame();
                            _gameFacade.PlayGameOverSound();
                            _gameFacade.RenderAllPlayers(_session.GetAllPlayers());
                            await _hubContext.Clients.All.SendAsync("GameEnded", new {
                                winner = _session.GetWinner()?.Name,
                                playerScores = _session.Players.ToDictionary(p => p.Value.Name, p => p.Value.Score)
                            });
                            await SendScoreboardUpdate();
                            break;
                        }
                    }
                    await Task.Delay(10);
                }

            });
            await SendScoreboardUpdate();
        }

        public async Task MoveBoatTo(float positionX)
        {
            var player = _session.GetPlayer(Context.ConnectionId);

            player.UpdateBoatPosition(positionX);

            await Clients.All.SendAsync("BoatMovedTo", player);

            //Console.WriteLine($"Player {Context.ConnectionId} moved to {positionX}");
        }
        public async Task ToggleFishingRodCast()
        {
            var player = _session.GetPlayer(Context.ConnectionId);

            // Update hook position
            player.ToggleFishingRodCast();

            await Clients.All.SendAsync("FishingRodCastChanged", player);

            Console.WriteLine($"Player {Context.ConnectionId} has toggled his cast");
        }
        public async Task MoveHook(float positionX, float positionY)
        {
            var player = _session.GetPlayer(Context.ConnectionId);

            if (player == null) return;

            // Update hook position
            player.FishingRod.PositionX = positionX;
            player.FishingRod.PositionY = positionY;

            await Clients.All.SendAsync("HookMovedTo", player);

            Console.WriteLine($"Player {Context.ConnectionId} hook moved to {positionX} {positionY}");
        }
        public async Task CatchFish(int fishId)
        {
            var player = _session.GetPlayer(Context.ConnectionId);
            var fish = _session.Environment.Fishes.FirstOrDefault(f => f.Id == fishId);

            if (fish != null && player != null)
            {
                // Use Facade to handle fish catch logic
                bool caught = _gameFacade.AttemptFishCatch(player, fish);

                if (!caught)
                {
                    double escapeChance = fish.Behavior?.GetEscapeProbability() ?? 0.5;
                    Console.WriteLine($"🐟 Fish {fishId} escaped! ({escapeChance * 100}% escape probability)");
                    // Send miss sound to frontend
                    await Clients.All.SendAsync("PlaySound", "miss");
                }
                else
                {
                    var decorator = fish.Decorator;
                    if (decorator != null)
                    {
                        _gameFacade.ApplyEffect(player, decorator);
                        // Send catch sound for decorated fish
                        await Clients.All.SendAsync("PlaySound", "catch");

                        if (decorator.CausesFreeze())
                        {
                            double penalty = decorator.GetPointsPenalty();
                            int deductedPoints = (int)Math.Round(fish.Points * penalty);
                            player.IsFrozen = true;
                            player.FreezeEndTime = DateTime.UtcNow.AddSeconds(decorator.GetFreezeDurationSeconds());
                            Console.WriteLine($"Poisoned fish caught! -{deductedPoints} points, hook frozen for {decorator.GetFreezeDurationSeconds()}s");
                            // Send freeze sound
                            await Clients.All.SendAsync("PlaySound", "freeze");
                        }
                        else if (decorator.GetSlowdownPercentage() > 0)
                        {
                            player.IsSlowed = true;
                            player.SlowdownEndTime = DateTime.UtcNow.AddSeconds(decorator.GetSlowdownDurationSeconds());
                            Console.WriteLine($"Weighted fish caught! Slowed for {decorator.GetSlowdownDurationSeconds()}s");
                        }
                        else
                        {
                            if (fish is BombFish)
                            {
                                new ResetScoreCommand().Execute(player);
                                Console.WriteLine($"💣 BombFish caught! Score reset!");
                                // Send bomb sound
                                await Clients.All.SendAsync("PlaySound", "bomb");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"✅ Fish caught! +{fish.Points} points");
                        // Send normal catch sound
                        await Clients.All.SendAsync("PlaySound", "catch");
                    }
                }

                _session.Environment.DeleteFish(fishId);
                await Clients.All.SendAsync("UpdateFishes", _session.Environment.Fishes);
                await Clients.All.SendAsync("PlayerUpdated", player);
                await SendScoreboardUpdate();
            }
        }
        public async Task SendScoreboardUpdate()
        {
            var session = Session.Instance;
            int remainingTime = session.TimerDuration;
            if (session.IsActive && session.State == GameState.Playing)
            {
                var elapsedSeconds = (int)(DateTime.UtcNow - session.StartTime).TotalSeconds;
                remainingTime = Math.Max(0, session.TimerDuration - elapsedSeconds);
            }
            var scoreboardData = new {
                playerScores = session.Players.ToDictionary(p => p.Value.Name, p => p.Value.Score),
                currentGameState = session.State.ToString(),
                remainingTime = remainingTime
            };
            
            await Clients.All.SendAsync("ScoreboardUpdated", scoreboardData);
        }

        public async Task ResetGame()
        {
            Console.WriteLine("Resetting game session...");
            
            foreach (var player in _session.Players.Values)
            {
                player.Score = 0;
                player.FishesPulledIn = 0;
            }
            _session.State = GameState.Waiting;
            _session.IsActive = false;
            _session.StartTime = DateTime.UtcNow;
            _session.EndTime = null;
            GameEnvironmentFactory factory = new SeaWaterEnvironmentFactory();
            _session.Environment = factory.getEnvironment();
            _gameFacade.PlaySuccessSound();
            _gameFacade.InitializeGame();
            
            await Clients.All.SendAsync("GameReset");
            await SendScoreboardUpdate();
            Console.WriteLine("✅ Game reset complete!");
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _session.RemovePlayer(Context.ConnectionId);
            await Clients.Others.SendAsync("PlayerLeft", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}
