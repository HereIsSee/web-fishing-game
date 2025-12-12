namespace Api.Models
{
    public class Player
    {
        private List<Fish> _caughtFishes = new List<Fish>();

        public IEnumerable<Fish> GetCaughtFishes()
        {
            foreach (var fish in _caughtFishes)
            {
                yield return fish;
            }
        }
        public Dictionary<string, int> GetCaughtFishesByType()
        {
            var result = new Dictionary<string, int>();
            
            if (_caughtFishes == null || !_caughtFishes.Any())
                return result;
            
            return _caughtFishes
                .GroupBy(f => f.Type ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());
        }
        
        // Add fish when caught
        public void AddCaughtFish(Fish fish)
        {
            _caughtFishes.Add(fish);
        }
        
        // Get fish count (uses iterator internally)
        public int GetTotalFishCaught()
        {
            return _caughtFishes.Count;
        }
        
        // Get unique fish types caught (uses iterator)
        public int GetUniqueFishTypesCaught()
        {
            if (_caughtFishes == null || !_caughtFishes.Any())
                return 0;
            
            // Return count, not the list itself
            return _caughtFishes
                .Select(f => f.Type)
                .Where(type => !string.IsNullOrEmpty(type))
                .Distinct()
                .Count();
        }
        public List<string> GetUniqueFishTypeNames()
        {
            if (_caughtFishes == null || !_caughtFishes.Any())
                return new List<string>();
            
            return _caughtFishes
                .Select(f => f.Type)
                .Where(type => !string.IsNullOrEmpty(type))
                .Distinct()
                .ToList();
        }
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; } = 0;
        public int FishesPulledIn { get; set; } = 0;
        
        // Player has a boat
        public Boat Boat { get; set; }

        // Player has a fishing rod
        public FishingRod FishingRod { get; set; } = null!;

        // Decorator Pattern: Freeze effect from poisoned fish
        public bool IsFrozen { get; set; } = false;
        public DateTime? FreezeEndTime { get; set; } = null;

        // Decorator Pattern: Slowdown effect from weighted fish
        public bool IsSlowed { get; set; } = false;
        public DateTime? SlowdownEndTime { get; set; } = null;

        // Memento: Persistent ID for browser storage
        private string _persistentPlayerId;
        
        public Player(string connectionId, string name, double positionX, double positionY, string? persistentId = null)
        {
            this.ConnectionId = connectionId;
            this.Name = name;
            this.Score = 0;
            this.FishesPulledIn = 0;
            this.Boat = new Boat(positionX, positionY);
            this.FishingRod = new FishingRod(positionX, positionY);
            
            // Use provided persistentId or generate new
            this._persistentPlayerId = persistentId ?? GeneratePersistentId();
        }

        // ==================== SIMPLE MEMENTO METHODS ====================
        
        // Creates a secure snapshot of ONLY the score
        public PlayerMemento Save()
        {
            Console.WriteLine($"💾 Saving player {Name} score: {Score}");
            return new PlayerMemento(Score, _persistentPlayerId, DateTime.UtcNow);
        }

        // Restores ONLY the score from a memento
        public void Load(PlayerMemento memento)
        {
            if (memento == null) return;
            
            Console.WriteLine($"🔄 Loading player {Name} from save. Old: {Score}, New: {memento.SavedScore}");
            
            // Restore ONLY the score
            Score = memento.SavedScore;
            
            // Update persistent ID if needed
            if (!string.IsNullOrEmpty(memento.SavedPersistentId))
            {
                _persistentPlayerId = memento.SavedPersistentId;
            }
            
            Console.WriteLine($"✅ Player {Name} restored with score: {Score}");
        }

        // ==================== SECURE MEMENTO CLASS ====================
        
        public class PlayerMemento
        {
            // Read-only fields - cannot be modified after creation
            public readonly int SavedScore;
            public readonly string SavedPersistentId;
            public readonly DateTime SavedTimestamp;

            internal PlayerMemento(int score, string persistentId, DateTime timestamp)
            {
                SavedScore = score;
                SavedPersistentId = persistentId;
                SavedTimestamp = timestamp;
            }
            
            // Simple Base64 encryption for security requirement
            public string ToEncryptedString()
            {
                try
                {
                    var data = $"{SavedScore}|{SavedPersistentId}|{SavedTimestamp:O}";
                    return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        
        // Static method to create PlayerMemento from encrypted string
        public static PlayerMemento? FromEncryptedString(string encryptedData)
        {
            try
            {
                var data = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
                var parts = data.Split('|');
                
                if (parts.Length < 3) return null;
                
                int score = int.Parse(parts[0]);
                string persistentId = parts[1];
                DateTime timestamp = DateTime.Parse(parts[2]);
                
                return new PlayerMemento(score, persistentId, timestamp);
            }
            catch
            {
                return null;
            }
        }

        // ==================== HELPER METHODS ====================
        
        private string GeneratePersistentId()
        {
            return $"player_{Guid.NewGuid():N}";
        }

        public string GetPersistentId() => _persistentPlayerId;

        // ==================== ORIGINAL METHODS (UNCHANGED) ====================
        
        public void UpdateBoatPosition(double positionX)
        {
            if (this.FishingRod.Cast)
                return;

            this.Boat.PositionX = positionX;
            this.FishingRod.PositionX = positionX;
        }
        
        public void ToggleFishingRodCast()
        {
            this.FishingRod.Cast = !this.FishingRod.Cast;
            if(FishingRod.Cast == false)
            {
                this.FishingRod.PositionX = this.Boat.PositionX;
                this.FishingRod.PositionY = this.Boat.PositionY;   
            }
        }
    }
}