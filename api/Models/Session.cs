using System.Collections.Concurrent;

namespace Api.Models
{
    public class Session
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; } = true;

        // Session has a Game 1 to 1
        public Game Game { get; set; } = new Game();

        // PRIDĖTI: Žaidėjų sąrašas
        public ConcurrentDictionary<string, Player> Players { get; } = new();

        // PRIDĖTI: Žaidimo būsenos
        public GameState State { get; set; } = GameState.Waiting;
        public int TimerDuration { get; set; } = 300; // 5 minučių

        // PRIDĖTI: Metodai žaidėjų valdymui
        public void AddPlayer(string connectionId, string playerName)
        {
            Players[connectionId] = new Player
            {
                ConnectionId = connectionId,
                Name = playerName,
                Score = 0,
                SessionId = this.Id, // Susiejame žaidėją su sesija
                Boat = new Boat
                {
                    PositionX = 0,
                    PositionY = 0,
                    MovementSpeed = 0,
                }
            };
        }

        public Player GetPlayer(string connectionId)
        {
            return Players[connectionId];
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
            Players.TryRemove(connectionId, out _);
        }

        public void StartGame()
        {
            State = GameState.Playing;
            IsActive = true;
            // Čia galima inicijuoti Game objektą jei reikia
        }

        public void EndGame()
        {
            State = GameState.Finished;
            EndTime = DateTime.UtcNow;
            IsActive = false;
        }

        // PRIDĖTI: Surasti laimėtoją
        public Player? GetWinner()
        {
            return Players.Values.OrderByDescending(p => p.Score).FirstOrDefault();
        }
    }

    // PRIDĖTI: Enum būsenoms
    public enum GameState
    {
        Waiting,
        Playing,
        Finished
    }
}