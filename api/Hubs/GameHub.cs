using Api.Models;
using Api.Models.Facade;
using Api.Models.Flyweight;
using Api.Models.Proxy;
using Api.Models.ChainOfResponsibility;
using Api.Models.Visitor;
using Api.Models.Mediator;
using Microsoft.AspNetCore.SignalR;
using Api.Models.Dto;


namespace Api.Hubs
{
    public class GameHub : Hub
    {
        private static Session _session = Session.Instance;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly ILogger<GameHub> _logger;
        
        // PROXY PATTERN: Wrap GameFacade with performance monitoring proxy (STATIC - created once)
        private static readonly MetricsGameFacadeProxy _gameFacade = new MetricsGameFacadeProxy(new GameFacade());
        
        // CHAIN OF RESPONSIBILITY: Validates catch attempts through 5 handlers (STATIC - created once)
        private static readonly CatchValidationChain _catchChain = new CatchValidationChain();
        
        // MEDIATOR PATTERN: Coordinates Player, Scoreboard, AudioSubsystem
        private static readonly GameEventMediator _mediator = new GameEventMediator();
        private static bool _mediatorInitialized = false;

        // PROXY PATTERN (Security): only host can reset/end/start
        private static SessionAccessControlProxy? _sessionSecurityProxy = null;

        public GameHub(IHubContext<GameHub> hubContext, ILogger<GameHub> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
            
            // MEDIATOR: Register components once
            if (!_mediatorInitialized)
            {
                _mediator.RegisterScoreboard(_session.Scoreboard);
                _mediator.RegisterAudioSubsystem(new VirtualAudioSubsystemProxy());
                _mediatorInitialized = true;
                Console.WriteLine("🎯 Mediator initialized with Scoreboard and AudioSubsystem");
            }
        }

        // ==================== ENHANCED JOIN WITH MEMENTO ====================
        public async Task JoinSession(string playerName, string? persistentId = null)
        {
            // CHECK: Prevent same connection from joining twice
            var existingPlayer = _session.GetPlayer(Context.ConnectionId);
            if (existingPlayer != null)
            {
                Console.WriteLine($"⚠️ Player {existingPlayer.Name} already connected (ID: {Context.ConnectionId})");

                // Just send existing data, don't create new player
                await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);
                await Clients.Caller.SendAsync("ReceiveAllPlayers", _session.GetAllPlayers());
                await Clients.Caller.SendAsync("ReceivePersistentId", existingPlayer.GetPersistentId());
                return;
            }

            Console.WriteLine($"🎣 Player {playerName} joining session...");
            Console.WriteLine($"🔑 Persistent ID: {persistentId ?? "None"}");

            // 1. FIRST add player to session
            _session.AddPlayer(Context.ConnectionId, playerName, persistentId);

            // 2. THEN get the player (now it exists!)
            var player = _session.GetPlayer(Context.ConnectionId);

            var allPlayers = _session.GetAllPlayers();
            await Clients.Caller.SendAsync("ReceiveAllPlayers", allPlayers);
            await Clients.All.SendAsync("PlayerJoined", player);
            await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);

            await SendGameEnvironmentToCaller();

            // Send persistent ID back to frontend for localStorage
            var playerPersistentId = player.GetPersistentId();
            await Clients.Caller.SendAsync("ReceivePersistentId", playerPersistentId);

            _gameFacade.RenderAllPlayers(allPlayers);
            
            // MEDIATOR: Register player and notify PlayerJoined event
            _mediator.RegisterPlayer(player);
            var playerJoinedEvent = new GameEvent("PlayerJoined");
            playerJoinedEvent.AddData("PlayerName", player.Name);
            playerJoinedEvent.AddData("ConnectionId", Context.ConnectionId);
            _mediator.Notify(this, playerJoinedEvent);

            Console.WriteLine($"✅ Player {playerName} joined with score: {player.Score}!");
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

        // ==================== SIMPLE MEMENTO ENDPOINTS ====================
        private async Task SendGameEnvironmentToCaller()
        {
            _session.Environment.InitializeEnvironmentFeatures();
            var dto = GameEnvironmentDtoFactory.FromEnvironment(_session.Environment);
            await Clients.Caller.SendAsync("GameEnvironmentData", dto);
        }

        
        public async Task SaveScore()
        {
            try
            {
                _session.SavePlayerScore(Context.ConnectionId);
                var player = _session.GetPlayer(Context.ConnectionId);
                
                // Get encrypted save for browser
                var encryptedSave = _session.GetSaveForBrowser(Context.ConnectionId);
                
                await Clients.Caller.SendAsync("ScoreSaved", new {
                    success = true,
                    score = player?.Score ?? 0,
                    playerName = player?.Name,
                    encryptedData = encryptedSave,
                    timestamp = DateTime.UtcNow
                });
                
                Console.WriteLine($"💾 Score saved for {player?.Name}: {player?.Score}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving score");
                await Clients.Caller.SendAsync("SaveFailed", "Failed to save score");
            }
        }

        public async Task LoadScore()
        {
            try
            {
                bool success = _session.LoadPlayerScore(Context.ConnectionId);
                
                if (success)
                {
                    var player = _session.GetPlayer(Context.ConnectionId);
                    
                    await Clients.Caller.SendAsync("ScoreLoaded", new {
                        success = true,
                        score = player?.Score ?? 0,
                        playerName = player?.Name,
                        timestamp = DateTime.UtcNow
                    });
                    
                    // Update scoreboard for everyone
                    await SendScoreboardUpdate();
                    await Clients.All.SendAsync("PlayerUpdated", player);
                    
                    Console.WriteLine($"🔄 Score loaded for {player?.Name}: {player?.Score}");
                }
                else
                {
                    await Clients.Caller.SendAsync("LoadFailed", "No saved score found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading score");
                await Clients.Caller.SendAsync("LoadFailed", $"Error: {ex.Message}");
            }
        }

        public async Task ImportScoreFromBrowser(string encryptedData)
        {
            try
            {
                bool success = _session.ImportSaveFromBrowser(Context.ConnectionId, encryptedData);
                
                if (success)
                {
                    await Clients.Caller.SendAsync("ScoreImported", new {
                        success = true,
                        timestamp = DateTime.UtcNow
                    });
                    Console.WriteLine($"📥 Score imported for {Context.ConnectionId}");
                }
                else
                {
                    await Clients.Caller.SendAsync("ImportFailed", "Invalid save data");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing score");
                await Clients.Caller.SendAsync("ImportFailed", $"Error: {ex.Message}");
            }
        }

        
        public async Task LeaveSession()
        {
            var player = _session.GetPlayer(Context.ConnectionId);
            if (player == null) 
            {
                Console.WriteLine($"⚠️ Player already left: {Context.ConnectionId}");
                return; // Already left
            }
            
            Console.WriteLine($"🚪 Player {player.Name} leaving...");
            
            // Auto-save final score
            _session.SavePlayerScore(Context.ConnectionId);
            
            // Tell frontend to clear the active browser flag
            await Clients.Caller.SendAsync("ClearActivePlayer", player.Name);
            
            // Remove from session FIRST
            _session.RemovePlayer(Context.ConnectionId);

            // If everyone left, clear host proxy so next session can pick a new host
            if (_session.Players.Count == 0)
            {
                _sessionSecurityProxy = null;
            }
            
            // THEN notify others
            await Clients.Others.SendAsync("PlayerLeft", Context.ConnectionId);
            await SendScoreboardUpdate();
            
            Console.WriteLine($"✅ Player {player.Name} left successfully");
        }
        
        public async Task StartGame()
        {
            var requesterPlayer = _session.GetPlayer(Context.ConnectionId);
            var requesterId = requesterPlayer?.GetPersistentId() ?? Context.ConnectionId;

            // First start defines the host. Subsequent StartGame calls must be by host.
            _sessionSecurityProxy ??= new SessionAccessControlProxy(_session, requesterId);

            try
            {
                _sessionSecurityProxy.StartGame(requesterId);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("ActionDenied", ex.Message);
                return;
            }

            _session.StartGame();
            _gameFacade.ResetMetrics();
            _gameFacade.InitializeGame();
            await Clients.All.SendAsync("GameStarted", _session.TimerDuration);
            
            // MEDIATOR: Notify GameStarted
            var gameStartedEvent = new GameEvent("GameStarted");
            gameStartedEvent.AddData("Duration", _session.TimerDuration);
            _mediator.Notify(this, gameStartedEvent);
            
            _ = Task.Run(async () =>
            {
                while (_session.IsActive)
                {
                    _session.Environment.Update();
                    await _hubContext.Clients.All.SendAsync("UpdateFishes", _session.Environment.GetAllFishesFlat());

                            
                    if (_session.State == GameState.Playing)
                    {
                        var elapsedSeconds = (int)(DateTime.UtcNow - _session.StartTime).TotalSeconds;
                        if (elapsedSeconds >= _session.TimerDuration)
                        {
                            Console.WriteLine("⏰ Game time expired! Ending game...");
                            _session.EndGame(); // Auto-saves all scores
                            _gameFacade.PlayGameOverSound();
                            _gameFacade.RenderAllPlayers(_session.GetAllPlayers());
                            
                            // PROXY PATTERN: Print performance metrics at game end
                            Console.WriteLine("\n📊 PERFORMANCE METRICS (Proxy Pattern):");
                            Console.WriteLine(_gameFacade.GetFormattedSummary());
                                    
                            // Send game ended notification
                            await _hubContext.Clients.All.SendAsync("GameEnded", new {
                                winner = _session.GetWinner()?.Name,
                                playerScores = _session.Players.ToDictionary(p => p.Value.Name, p => p.Value.Score),

                                // PROXY PATTERN: send performance report to frontend
                                performanceStats = new {
                                    facadeOperations = _gameFacade.GetPerformanceReport()
                                }
                            });
                            
                            // 🎣 NEW: Send fish collection to each player
                            foreach (var connectionId in _session.Players.Keys)
                            {
                                await Clients.Client(connectionId).SendAsync("ShowPlayerFishCollection");
                            }
                            
                            await SendScoreboardUpdate();
                            break;
                        }
                    }
                    await Task.Delay(10);
                }
            });
            
            await SendScoreboardUpdate();
        }

        public async Task CatchFish(int fishId)
        {
            var player = _session.GetPlayer(Context.ConnectionId);
            var fish = _session.Environment.GetAllFishesFlat().FirstOrDefault(f => f.Id == fishId);


            if (fish != null && player != null)
            {
                // CHAIN OF RESPONSIBILITY: Validate catch attempt through 5 handlers
                var catchContext = _catchChain.ProcessCatchAttempt(player, fish, _session);
                
                if (!catchContext.IsValid)
                {
                    Console.WriteLine($"❌ Catch rejected by Chain: {catchContext.FailureReason}");
                    await Clients.Caller.SendAsync("CatchFailed", catchContext.FailureReason);
                    return;
                }
                
                bool caught = _gameFacade.AttemptFishCatch(player, fish);

                if (!caught)
                {
                    double escapeChance = fish.Behavior?.GetEscapeProbability() ?? 0.5;
                    Console.WriteLine($"🐟 Fish {fishId} escaped! ({escapeChance * 100}% escape probability)");
                    await Clients.All.SendAsync("PlaySound", "miss");
                }
                else
                {
                    // VISITOR PATTERN: Use visitors to calculate score and determine sound
                    var scoreVisitor = new ScoreVisitor();
                    var soundVisitor = new SoundVisitor();
                    
                    fish.Accept(scoreVisitor);
                    fish.Accept(soundVisitor);
                    
                    // Apply visitor-calculated score instead of hardcoded fish.Points
                    int pointsAwarded = scoreVisitor.CalculatedScore;
                    player.Score += pointsAwarded;
                    Console.WriteLine($"🎯 VISITOR: Awarded {pointsAwarded} points (breakdown: {scoreVisitor.ScoreBreakdown})");
                    
                    player.AddCaughtFish(fish); // Add to player's collection
                    player.FishesPulledIn++; // Increment count
                    
                    // MEDIATOR: Notify FishCaught event
                    var fishCaughtEvent = new GameEvent("FishCaught");
                    fishCaughtEvent.AddData("PlayerName", player.Name);
                    fishCaughtEvent.AddData("FishType", fish.Type);
                    fishCaughtEvent.AddData("Points", pointsAwarded);
                    _mediator.Notify(this, fishCaughtEvent);
                    
                    var decorator = fish.Decorator;
                    if (decorator != null)
                    {
                        _gameFacade.ApplyEffect(player, decorator);
                        await Clients.All.SendAsync("PlaySound", soundVisitor.SoundCategory);

                        if (decorator.CausesFreeze())
                        {
                            double penalty = decorator.GetPointsPenalty();
                            int deductedPoints = (int)Math.Round(fish.Points * penalty);
                            player.IsFrozen = true;
                            player.FreezeEndTime = DateTime.UtcNow.AddSeconds(decorator.GetFreezeDurationSeconds());
                            Console.WriteLine($"Poisoned fish caught! -{deductedPoints} points, hook frozen for {decorator.GetFreezeDurationSeconds()}s");
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
                                await Clients.All.SendAsync("PlaySound", "bomb");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"✅ Fish caught! +{pointsAwarded} points");
                        await Clients.All.SendAsync("PlaySound", soundVisitor.SoundEffect);
                    }

                    // AUTO-SAVE: After catching fish
                    _session.SavePlayerScore(Context.ConnectionId);
                    Console.WriteLine($"💾 Auto-saved after fish catch: {player.Score}");
                }

                // Change nearby fish state to scared
                UpdateFishStateToScared(fish, fishId);

                _session.Environment.DeleteFish(fishId);
                await Clients.All.SendAsync("UpdateFishes", _session.Environment.GetAllFishesFlat());
                await Clients.All.SendAsync("PlayerUpdated", player);
                await SendScoreboardUpdate();
            }
        }
        public void UpdateFishStateToScared(Fish fish, int fishId)
        {
            const double scareRadius = 300.0;
            var scareRadiusSq = scareRadius * scareRadius;

            foreach (var group in _session.Environment.FishGroups)
            {
                foreach (var f in group.Flatten())
                {
                    if (f.Id == fishId) continue;

                    var dx = f.PositionX - fish.PositionX;
                    var dy = f.PositionY - fish.PositionY;

                    if (dx*dx + dy*dy <= scareRadiusSq)
                    {
                        group.TriggerScare();
                        break;
                    }
                }
            }

        }

        // ==================== ORIGINAL METHODS UNCHANGED ====================
        public async Task MoveBoatTo(float positionX)
        {
            var player = _session.GetPlayer(Context.ConnectionId);
            player.UpdateBoatPosition(positionX);
            await Clients.All.SendAsync("BoatMovedTo", player);
        }

        public async Task ToggleFishingRodCast()
        {
            var player = _session.GetPlayer(Context.ConnectionId);
            player.ToggleFishingRodCast();
            await Clients.All.SendAsync("FishingRodCastChanged", player);
            Console.WriteLine($"Player {Context.ConnectionId} has toggled his cast");
        }

        public async Task MoveHook(float positionX, float positionY)
        {
            var player = _session.GetPlayer(Context.ConnectionId);
            if (player == null) return;

            player.FishingRod.PositionX = positionX;
            player.FishingRod.PositionY = positionY;
            await Clients.All.SendAsync("HookMovedTo", player);
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
            
            // FIX FOR DUPLICATE NAMES: Use connection ID in key
            var playerScores = new Dictionary<string, int>();
            foreach (var kvp in session.Players)
            {
                var player = kvp.Value;
                // Unique key: name + short connection ID
                var key = $"{player.Name} ({kvp.Key.Substring(0, 4)})";
                playerScores[key] = player.Score;
            }
            
            var scoreboardData = new {
                playerScores = playerScores,
                currentGameState = session.State.ToString(),
                remainingTime = remainingTime
            };
            
            await Clients.All.SendAsync("ScoreboardUpdated", scoreboardData);
        }

        public async Task ResetGame()
        {
            Console.WriteLine("Resetting game session...");

            var requesterPlayer = _session.GetPlayer(Context.ConnectionId);
            var requesterId = requesterPlayer?.GetPersistentId() ?? Context.ConnectionId;

            if (_sessionSecurityProxy == null)
            {
                var msg = "❌ DENIED: No host has been established for this session.";
                Console.WriteLine(msg);
                await Clients.Caller.SendAsync("ActionDenied", msg);
                return;
            }

            try
            {
                _sessionSecurityProxy.ResetGame(requesterId);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("ActionDenied", ex.Message);
                return;
            }
            
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
            _gameFacade.ResetMetrics();
            _gameFacade.InitializeGame();
            
            await Clients.All.SendAsync("GameReset");
            await SendScoreboardUpdate();

            var envDto = GameEnvironmentDtoFactory.FromEnvironment(_session.Environment);
            await Clients.All.SendAsync("GameEnvironmentData", envDto);

            Console.WriteLine("✅ Game reset complete!");
        }

        // Host-only: allow ending the game early
        public async Task EndGameEarly()
        {
            var requesterPlayer = _session.GetPlayer(Context.ConnectionId);
            var requesterId = requesterPlayer?.GetPersistentId() ?? Context.ConnectionId;

            if (_sessionSecurityProxy == null)
            {
                var msg = "❌ DENIED: No host has been established for this session.";
                Console.WriteLine(msg);
                await Clients.Caller.SendAsync("ActionDenied", msg);
                return;
            }

            try
            {
                _sessionSecurityProxy.EndGame(requesterId);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("ActionDenied", ex.Message);
                return;
            }

            // Match the normal end-of-game notifications
            _gameFacade.PlayGameOverSound();
            _gameFacade.RenderAllPlayers(_session.GetAllPlayers());

            await _hubContext.Clients.All.SendAsync("GameEnded", new
            {
                winner = _session.GetWinner()?.Name,
                playerScores = _session.Players.ToDictionary(p => p.Value.Name, p => p.Value.Score),
                performanceStats = new
                {
                    facadeOperations = _gameFacade.GetPerformanceReport()
                }
            });

            foreach (var connectionId in _session.Players.Keys)
            {
                await Clients.Client(connectionId).SendAsync("ShowPlayerFishCollection");
            }

            await SendScoreboardUpdate();
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"🎯 [BACKEND] OnDisconnectedAsync triggered - ConnectionId: {Context.ConnectionId}");
            
            // Get player before removing
            var player = _session.GetPlayer(Context.ConnectionId);
            
            if (player != null)
            {
                Console.WriteLine($"🎯 [BACKEND] Player {player.Name} disconnected - clearing active status");
                // Tell frontend to clear localStorage flags
                await Clients.Caller.SendAsync("ClearActivePlayer", player.Name);
            }
            
            // Continue with your existing LeaveSession logic
            await LeaveSession();
            await base.OnDisconnectedAsync(exception);
        }
        public async Task TestFlyweightPerformance()
        {
            Console.WriteLine("🎣 Running Flyweight Pattern Performance Measurement...");
            
            // Run your test
            FishFlyweightFactory.TestPerformance();
            FishFlyweightFactory.PrintMemoryStatistics();
            
            await Clients.Caller.SendAsync("TestResult", "Flyweight performance test completed!");
        }
        public async Task RunFlyweightTest()
        {
            Console.WriteLine("🧪 Running Flyweight Performance Test...");
            
            // Run your test
            Api.Models.Flyweight.FishFlyweightFactory.TestPerformance();
            
            await Clients.Caller.SendAsync("TestComplete", "Flyweight test finished!");
        }
        
        public async Task ShowPlayerFishCollection()
        {
            var player = _session.GetPlayer(Context.ConnectionId);
            if (player == null) 
            {
                Console.WriteLine($"⚠️ Player not found for fish collection: {Context.ConnectionId}");
                return;
            }
            
            try
            {
                // Get the fish by type - this now returns Dictionary<string, int>
                var fishByTypeDict = player.GetCaughtFishesByType();
                
                // Create safe collections
                var allFish = new List<object>();
                var caughtFishes = player.GetCaughtFishes();
                
                if (caughtFishes != null)
                {
                    foreach (var fish in caughtFishes)
                    {
                        allFish.Add(new 
                        { 
                            Type = fish.Type ?? "Unknown", 
                            Points = fish.Points,
                            Color = fish.Color ?? "#000000",
                            Name = fish.Type ?? "Unknown"
                        });
                    }
                }
                
                var fishStats = new
                {
                    TotalCaught = player.GetTotalFishCaught(),
                    UniqueTypes = player.GetUniqueFishTypesCaught(),
                    FishByType = fishByTypeDict, // This is now a Dictionary<string, int>
                    AllFish = allFish,
                    PlayerName = player.Name
                };
                
                Console.WriteLine($"🎣 Preparing fish collection for {player.Name}:");
                Console.WriteLine($"   Total: {fishStats.TotalCaught}");
                Console.WriteLine($"   Unique types: {fishStats.UniqueTypes}");
                Console.WriteLine($"   FishByType entries: {fishStats.FishByType.Count}");
                
                // Log some details for debugging
                foreach (var kvp in fishStats.FishByType)
                {
                    Console.WriteLine($"   - {kvp.Key}: {kvp.Value}");
                }
                
                await Clients.Caller.SendAsync("FishCollection", fishStats);
                Console.WriteLine($"🎣 Sent fish collection to {player.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error preparing fish collection: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                
                // Send minimal valid data
                await Clients.Caller.SendAsync("FishCollection", new
                {
                    TotalCaught = 0,
                    UniqueTypes = 0,
                    FishByType = new Dictionary<string, int>(),
                    AllFish = new List<object>(),
                    PlayerName = player?.Name ?? "Unknown"
                });
            }
        }
    }
}