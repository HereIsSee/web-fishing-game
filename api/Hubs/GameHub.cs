using Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class GameHub : Hub
    {
        private static Session _session = Session.Instance;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly ILogger<GameHub> _logger;

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

            Console.WriteLine($"✅ Player {playerName} joined!");
            Console.WriteLine($"Player {Context.ConnectionId} connection id!");
            Console.WriteLine($"📊 Sent {allPlayers.Count} existing players to new player");

            Console.WriteLine(_session.IsActive);
            if (!_session.IsActive)
            {
                Console.WriteLine("irst player joined — starting game automatically!");
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
                Random random = new Random();
                double escapeChance = fish.Behavior?.GetEscapeProbability() ?? 0.5; // Default 50% if no behavior
                
                if (random.NextDouble() < escapeChance)
                {
                    Console.WriteLine($"🐟 Fish {fishId} escaped! ({escapeChance * 100}% escape probability)");
                }
                else
                {
                    var decorator = fish.Decorator;
                    
                    if (decorator != null)
                    {
                        if (decorator.CausesFreeze())
                        {
                            double penalty = decorator.GetPointsPenalty();
                            int deductedPoints = (int)Math.Round(fish.Points * penalty);
                            player.Score -= deductedPoints;
                            if (player.Score < 0) player.Score = 0;
                            player.IsFrozen = true;
                            player.FreezeEndTime = DateTime.UtcNow.AddSeconds(decorator.GetFreezeDurationSeconds());
                            Console.WriteLine($"Poisoned fish caught! -{deductedPoints} points, hook frozen for {decorator.GetFreezeDurationSeconds()}s");
                        }
                        else if (decorator.GetSlowdownPercentage() > 0)
                        {
                            double multiplier = decorator.GetPointsMultiplier();
                            int decoratedPoints = (int)Math.Round(fish.Points * multiplier);
                            player.Score += decoratedPoints;
                            player.IsSlowed = true;
                            player.SlowdownEndTime = DateTime.UtcNow.AddSeconds(decorator.GetSlowdownDurationSeconds());
                            Console.WriteLine($"Weighted fish caught! +{decoratedPoints} points ({multiplier}x), slowed for {decorator.GetSlowdownDurationSeconds()}s");
                        }
                        else
                        {
                            if (fish is BombFish)
                            {
                                new ResetScoreCommand().Execute(player);  
                                Console.WriteLine($"💣 BombFish caught! Score reset!");
                            }
                            else
                            {
                                player.Score += fish.Points;
                                Console.WriteLine($"✅ Normal fish caught! +{fish.Points} points");
                            }
                        }
                    }
                    else
                    {
                        player.Score += fish.Points;
                        Console.WriteLine($"✅ Fish caught! +{fish.Points} points");
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
            await Clients.All.SendAsync("GameReset");
            await SendScoreboardUpdate();
            Console.WriteLine("✅ Game reset complete!");
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _session.RemovePlayer(Context.ConnectionId);
            await Clients.Others.SendAsync("PlayerLeft", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}
