using System.Collections.Concurrent;

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
        public int TimerDuration { get; set; } = 300;

        public Session()
        {

            this.StartTime = DateTime.UtcNow;
            this.Environment = new GameEnvironment(800, 600, 500, 30, 10);
            this.IsActive = false;
        }

        public void AddPlayer(string connectionId, string playerName)
        {
            Random rnd = new Random();
            double positionX = rnd.Next(0, 800);

            Players[connectionId] = new Player(
                connectionId, playerName, positionX, 500.0
            );
        }

        public Player GetPlayer(string connectionId)
        {
            return Players[connectionId];
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