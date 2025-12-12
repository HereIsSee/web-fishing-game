using System.Collections.Concurrent;
using System.Security.Principal;
using Api.Hubs;
using Api.Models.Memento;

namespace Api.Models
{
    public class Session
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; } = false;

        public Scoreboard Scoreboard { get; set; }
        public GameEnvironment Environment { get; set; }
        public ConcurrentDictionary<string, Player> Players { get; } = new();

        public GameState State { get; set; } = GameState.Waiting;
        public int TimerDuration { get; set; } = 40;

        // ==================== SIMPLE MEMENTO INTEGRATION ====================
        private readonly SaveManager _saveManager = SaveManager.Instance;

        private static readonly Lazy<Session> _instance = new Lazy<Session>(() => new Session());
        public static Session Instance => _instance.Value;
        
        private Session()
        {
            GameEnvironmentFactory factory = new SeaWaterEnvironmentFactory();

            this.StartTime = DateTime.UtcNow;
            this.Environment = factory.getEnvironment();
            this.IsActive = false;

            this.Scoreboard = new Scoreboard();
            this.Attach(this.Scoreboard);
        }

        // ==================== OBSERVER PATTERN ====================
        private List<Observer> _observers = new List<Observer>();

        public void Attach(Observer observer) {
            _observers.Add(observer);
        }

        public void Detach(Observer observer) {
            _observers.Remove(observer);
        }

        private void Notify() {
            foreach (var observer in _observers) {
                observer.Update(this);
            }
        }

        // ==================== PLAYER MANAGEMENT WITH SCORE PERSISTENCE ====================
        
        public void AddPlayer(string connectionId, string playerName, string? persistentIdFromBrowser = null)
        {
            Random rnd = new Random();
            double positionX = rnd.Next(0, 800);

            var player = new Player(connectionId, playerName, positionX, 500.0, persistentIdFromBrowser);
            Players[connectionId] = player;
            
            // If browser provided a persistent ID, try to load saved score
            if (!string.IsNullOrEmpty(persistentIdFromBrowser))
            {
                // You need to add a SetPersistentId method to Player class
                // player.SetPersistentId(persistentIdFromBrowser);
                
                // Try to load saved score
                if (_saveManager.HasSave(persistentIdFromBrowser))
                {
                    _saveManager.LoadPlayerScore(player);
                    Console.WriteLine($"🔄 Loaded saved score {player.Score} for {playerName}");
                }
            }
            
            Notify();
            Console.WriteLine($"✅ Player {playerName} joined with score: {player.Score}");
        }

        public Player GetPlayer(string connectionId)
        {
            // Safer: TryGet instead of direct access
            if (Players.TryGetValue(connectionId, out var player))
            {
                return player;
            }
            return null; // Or throw a proper exception
        }

        public List<Player> GetAllPlayers()
        {
            return Players.Values.ToList();
        }

        public void UpdatePlayerPosition(string connectionId, double PositionX)
        {
            var player = Players[connectionId];
            if (player.Boat != null)
            {
                player.Boat.PositionX = PositionX;
            }
        }

        public void RemovePlayer(string connectionId)
        {
            // Auto-save score before player leaves
            if (Players.TryGetValue(connectionId, out var player))
            {
                _saveManager.SavePlayerScore(player);
                Console.WriteLine($"💾 Auto-saved score {player.Score} for {player.Name}");
            }
            
            Players.TryRemove(connectionId, out _);
            Notify();
        }

        // ==================== SIMPLE MEMENTO METHODS ====================
        
        public void SavePlayerScore(string connectionId)
        {
            if (Players.TryGetValue(connectionId, out var player))
            {
                _saveManager.SavePlayerScore(player);
            }
        }

        public bool LoadPlayerScore(string connectionId)
        {
            if (Players.TryGetValue(connectionId, out var player))
            {
                return _saveManager.LoadPlayerScore(player);
            }
            return false;
        }

        public string GetSaveForBrowser(string connectionId)
        {
            if (Players.TryGetValue(connectionId, out var player))
            {
                return _saveManager.GetSaveForBrowser(player.GetPersistentId());
            }
            return string.Empty;
        }

        public bool ImportSaveFromBrowser(string connectionId, string encryptedData)
        {
            if (Players.TryGetValue(connectionId, out var player))
            {
                return _saveManager.ImportSaveFromBrowser(player.GetPersistentId(), encryptedData);
            }
            return false;
        }

        // ==================== GAME MANAGEMENT ====================
        
        public void StartGame()
        {
            State = GameState.Playing;
            IsActive = true;
            Notify();
        }

        public void EndGame()
        {
            State = GameState.Finished;
            EndTime = DateTime.UtcNow;
            IsActive = false;
            
            // Auto-save all players' final scores
            foreach (var player in Players.Values)
            {
                _saveManager.SavePlayerScore(player);
            }
            
            Notify();
            Console.WriteLine("🏁 Game ended - all scores saved");
        }

        public Player? GetWinner()
        {
            return Players.Values.OrderByDescending(p => p.Score).FirstOrDefault();
        }
    }

    // Enum for game states
    public enum GameState
    {
        Waiting,
        Playing,
        Finished
    }
}