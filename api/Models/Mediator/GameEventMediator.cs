using Api.Models.Facade;

namespace Api.Models.Mediator
{
    /// <summary>
    /// Coordinates communication between Player, Scoreboard, and AudioSubsystem
    /// </summary>
    public class GameEventMediator : IGameMediator
    {
        private readonly List<Player> _players = new();
        private Scoreboard? _scoreboard;
        private IAudioSubsystem? _audioSubsystem;
        private readonly Dictionary<string, int> _eventCounts = new();

        public GameEventMediator()
        {
            Console.WriteLine("🎯 GameEventMediator created - ready to coordinate game components");
        }

        public void RegisterPlayer(Player player)
        {
            if (!_players.Contains(player))
            {
                _players.Add(player);
                Console.WriteLine($"🎯 Mediator: Registered player '{player.Name}'");
            }
        }

        public void RegisterScoreboard(Scoreboard scoreboard)
        {
            _scoreboard = scoreboard;
            Console.WriteLine("🎯 Mediator: Registered Scoreboard");
        }

        public void RegisterAudioSubsystem(IAudioSubsystem audioSubsystem)
        {
            _audioSubsystem = audioSubsystem;
            Console.WriteLine("🎯 Mediator: Registered AudioSubsystem");
        }

        public void Notify(object sender, GameEvent gameEvent)
        {
            // Track event statistics
            if (!_eventCounts.ContainsKey(gameEvent.EventType))
                _eventCounts[gameEvent.EventType] = 0;
            _eventCounts[gameEvent.EventType]++;

            Console.WriteLine($"\n🎯 MEDIATOR: Received '{gameEvent.EventType}' event from {sender.GetType().Name}");
            Console.WriteLine("   Coordinating response across components...");

            switch (gameEvent.EventType)
            {
                case "FishCaught":
                    HandleFishCaught(sender, gameEvent);
                    break;

                case "PlayerJoined":
                    HandlePlayerJoined(sender, gameEvent);
                    break;

                case "PlayerLeft":
                    HandlePlayerLeft(sender, gameEvent);
                    break;

                case "GameStarted":
                    HandleGameStarted(sender, gameEvent);
                    break;

                case "GameEnded":
                    HandleGameEnded(sender, gameEvent);
                    break;

                case "ScoreUpdated":
                    HandleScoreUpdated(sender, gameEvent);
                    break;

                default:
                    Console.WriteLine($"   ⚠️ Unknown event type: {gameEvent.EventType}");
                    break;
            }

            Console.WriteLine($"   ✅ Event '{gameEvent.EventType}' processed\n");
        }

        private void HandleFishCaught(object sender, GameEvent gameEvent)
        {
            var playerName = gameEvent.GetData<string>("PlayerName");
            var fishType = gameEvent.GetData<string>("FishType");
            var points = gameEvent.GetData<int>("Points");

            Console.WriteLine($"   🐟 Fish caught by {playerName}: {fishType} (+{points} points)");

            // 1. Update Scoreboard
            if (_scoreboard != null && playerName != null)
            {
                if (_scoreboard.PlayerScores.ContainsKey(playerName))
                {
                    _scoreboard.PlayerScores[playerName] += points;
                }
                else
                {
                    _scoreboard.PlayerScores[playerName] = points;
                }
                Console.WriteLine($"   📊 Mediator → Scoreboard: Updated {playerName}'s score");
            }

            // 2. Play catch sound via AudioSubsystem
            if (_audioSubsystem != null)
            {
                if (points > 40)
                {
                    _audioSubsystem.PlayCatchSound(); // Epic catch
                }
                else if (points < 0)
                {
                    _audioSubsystem.PlayMissSound(); // Bomb fish
                }
                else
                {
                    _audioSubsystem.PlayCatchSound(); // Normal catch
                }
                Console.WriteLine($"   🔊 Mediator → AudioSubsystem: Played catch sound");
            }

            // 3. Notify other players (could broadcast via SignalR hub)
            foreach (var player in _players)
            {
                if (player.Name != playerName)
                {
                    Console.WriteLine($"   📢 Mediator → Player '{player.Name}': Notified about {playerName}'s catch");
                }
            }
        }

        private void HandlePlayerJoined(object sender, GameEvent gameEvent)
        {
            var playerName = gameEvent.GetData<string>("PlayerName");
            Console.WriteLine($"   👋 New player joined: {playerName}");

            // Initialize player score on scoreboard
            if (_scoreboard != null && playerName != null)
            {
                _scoreboard.PlayerScores[playerName] = 0;
                Console.WriteLine($"   📊 Mediator → Scoreboard: Initialized score for {playerName}");
            }

            // Play welcome sound
            if (_audioSubsystem != null)
            {
                _audioSubsystem.PlayAmbientSound();
                Console.WriteLine($"   🔊 Mediator → AudioSubsystem: Played welcome sound");
            }
        }

        private void HandlePlayerLeft(object sender, GameEvent gameEvent)
        {
            var playerName = gameEvent.GetData<string>("PlayerName");
            Console.WriteLine($"   👋 Player left: {playerName}");

            // Keep score but mark as inactive
            Console.WriteLine($"   📊 Mediator → Scoreboard: Player {playerName} disconnected");

            // Notify remaining players
            foreach (var player in _players)
            {
                if (player.Name != playerName)
                {
                    Console.WriteLine($"   📢 Mediator → Player '{player.Name}': Notified about {playerName}'s departure");
                }
            }
        }

        private void HandleGameStarted(object sender, GameEvent gameEvent)
        {
            Console.WriteLine("   🎮 Game started!");

            // Reset scoreboard
            if (_scoreboard != null)
            {
                _scoreboard.PlayerScores.Clear();
                foreach (var player in _players)
                {
                    _scoreboard.PlayerScores[player.Name] = 0;
                }
                Console.WriteLine($"   📊 Mediator → Scoreboard: Reset scores for {_players.Count} players");
            }

            // Play game start sound
            if (_audioSubsystem != null)
            {
                _audioSubsystem.PlayAmbientSound();
                Console.WriteLine("   🔊 Mediator → AudioSubsystem: Played game start sound");
            }

            // Notify all players
            foreach (var player in _players)
            {
                Console.WriteLine($"   📢 Mediator → Player '{player.Name}': Game has started!");
            }
        }

        private void HandleGameEnded(object sender, GameEvent gameEvent)
        {
            Console.WriteLine("   🏁 Game ended!");

            // Determine winner from scoreboard
            if (_scoreboard != null && _scoreboard.PlayerScores.Any())
            {
                var winner = _scoreboard.PlayerScores.OrderByDescending(kv => kv.Value).First();
                Console.WriteLine($"   🏆 Winner: {winner.Key} with {winner.Value} points");
            }

            // Play game over sound
            if (_audioSubsystem != null)
            {
                _audioSubsystem.PlayMissSound();
                Console.WriteLine("   🔊 Mediator → AudioSubsystem: Played game over sound");
            }

            // Notify all players
            foreach (var player in _players)
            {
                Console.WriteLine($"   📢 Mediator → Player '{player.Name}': Game has ended!");
            }
        }

        private void HandleScoreUpdated(object sender, GameEvent gameEvent)
        {
            var playerName = gameEvent.GetData<string>("PlayerName");
            var newScore = gameEvent.GetData<int>("NewScore");

            Console.WriteLine($"   📊 Score updated: {playerName} → {newScore}");

            // Update scoreboard
            if (_scoreboard != null && playerName != null)
            {
                _scoreboard.PlayerScores[playerName] = newScore;
                Console.WriteLine($"   📊 Mediator → Scoreboard: Updated {playerName}'s score to {newScore}");
            }
        }

        public Dictionary<string, int> GetEventStatistics()
        {
            return new Dictionary<string, int>(_eventCounts);
        }

        public string GetMediatorReport()
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("\n╔════════════════════════════════════════════════════════════╗");
            report.AppendLine("║         MEDIATOR PATTERN - ACTIVITY REPORT                 ║");
            report.AppendLine("╚════════════════════════════════════════════════════════════╝");
            report.AppendLine($"\n📊 Registered Components:");
            report.AppendLine($"   Players: {_players.Count}");
            report.AppendLine($"   Scoreboard: {(_scoreboard != null ? "✅ Registered" : "❌ Not registered")}");
            report.AppendLine($"   AudioSubsystem: {(_audioSubsystem != null ? "✅ Registered" : "❌ Not registered")}");
            
            report.AppendLine($"\n📈 Event Statistics:");
            foreach (var kvp in _eventCounts.OrderByDescending(kv => kv.Value))
            {
                report.AppendLine($"   {kvp.Key}: {kvp.Value} times");
            }

            report.AppendLine("\n💡 Mediator Benefits:");
            report.AppendLine("   ✅ Components don't directly depend on each other");
            report.AppendLine("   ✅ Easy to add new components without modifying existing ones");
            report.AppendLine("   ✅ Centralized coordination logic");
            report.AppendLine("   ✅ Events are logged and tracked automatically");

            return report.ToString();
        }

        public string GetDifferenceFromObserver()
        {
            return @"
╔════════════════════════════════════════════════════════════════════╗
║              MEDIATOR vs OBSERVER - Key Differences                ║
╚════════════════════════════════════════════════════════════════════╝

🔷 OBSERVER PATTERN (already in your code: Session → Scoreboard):
   • One-to-many dependency (Subject → Observers)
   • Subject broadcasts changes to all observers
   • Observers subscribe/unsubscribe to subject
   • Subject doesn't know what observers do with the data
   • Loose coupling: observers are independent of each other
   • Example: Session.Notify() → all observers update themselves
   
🔶 MEDIATOR PATTERN (GameEventMediator):
   • Many-to-many communication reduced to one-to-many
   • Mediator coordinates interactions between components
   • Components don't know about each other, only the mediator
   • Mediator contains business logic for coordination
   • Tight coupling to mediator, but loose between components
   • Example: Player catches fish → Mediator → updates Scoreboard,
             plays sound, notifies other players

📋 WHEN TO USE:
   Observer: When one object's state changes should notify many others
   Mediator: When multiple objects need to communicate in complex ways
             and you want to avoid direct dependencies between them

🎮 IN YOUR GAME:
   Observer: Session notifies Scoreboard when game state changes
   Mediator: Coordinates Player actions → Scoreboard updates + Audio
             feedback + other player notifications (3+ components)
╚════════════════════════════════════════════════════════════════════╝";
        }
    }
}
