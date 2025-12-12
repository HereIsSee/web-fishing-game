using System.Collections.Concurrent;
using System.Text;

namespace Api.Models.Memento
{
    public class SaveManager
    {
        private static readonly Lazy<SaveManager> _instance = new Lazy<SaveManager>(() => new SaveManager());
        public static SaveManager Instance => _instance.Value;
        
        // Simple storage: PlayerPersistentId -> EncryptedMemento
        private ConcurrentDictionary<string, string> _playerSaves = new();
        
        private SaveManager() { }

        // ==================== SIMPLE SAVE/LOAD ====================
        
        public void SavePlayerScore(Player player)
        {
            Console.WriteLine($"🔍 DEBUG: Player {player.Name} score before save: {player.Score}");

            var memento = player.Save(); // Does this return correct score?
            Console.WriteLine($"🔍 DEBUG: Player {player.Name} score before save: {player.Score}");

            Console.WriteLine($"💾 Saving player {player.Name} state...");
            Console.WriteLine($"💾 Saved score {player.Score} for {player.Name}");
            
            // Make sure memento actually contains the score!
            var encrypted = memento.ToEncryptedString();
            _playerSaves[player.GetPersistentId()] = encrypted;
        }

        public bool LoadPlayerScore(Player player)
        {
            Console.WriteLine($"🔍 DEBUG: Loading for persistent ID: {player.GetPersistentId()}");

            if (!_playerSaves.TryGetValue(player.GetPersistentId(), out var encrypted))
                return false;
            
            var memento = Player.FromEncryptedString(encrypted);
            if (memento == null) return false;
            
            player.Load(memento);
            Console.WriteLine($"🔄 Loaded score {player.Score} for {player.Name}");
            return true;
        }

        // ==================== BROWSER STORAGE HELPERS ====================
        
        public string GetSaveForBrowser(string persistentId)
        {
            _playerSaves.TryGetValue(persistentId, out var encrypted);
            return encrypted ?? string.Empty;
        }

        public bool ImportSaveFromBrowser(string persistentId, string encryptedData)
        {
            try
            {
                // Validate it's a proper memento
                var test = Player.FromEncryptedString(encryptedData);
                if (test == null) return false;
                
                _playerSaves[persistentId] = encryptedData;
                Console.WriteLine($"📥 Imported save for player {persistentId}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool HasSave(string persistentId)
        {
            return _playerSaves.ContainsKey(persistentId);
        }

        public void ClearSave(string persistentId)
        {
            _playerSaves.TryRemove(persistentId, out _);
            Console.WriteLine($"🗑️ Cleared save for player {persistentId}");
        }
    }
}