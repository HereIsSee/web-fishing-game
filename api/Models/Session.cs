using System.Collections.Concurrent;
using System.Security.Principal;
using Api.Hubs;

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

        private static readonly Lazy<Session> _instance = new Lazy<Session>(() => new Session());
        public static Session Instance => _instance.Value;
        private Session()
        {

            this.StartTime = DateTime.UtcNow;
            this.Environment = new GameEnvironment(800, 600, 500, 30, 10);
            this.IsActive = false;

            this.Scoreboard = new Scoreboard();
            this.Attach(this.Scoreboard);
        }

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

        public void AddPlayer(string connectionId, string playerName)
        {
            Random rnd = new Random();
            double positionX = rnd.Next(0, 800);

            Players[connectionId] = new Player(
                connectionId, playerName, positionX, 500.0
            );
            Notify();
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
            Notify();
        }

        public void StartGame()
        {
            State = GameState.Playing;
            IsActive = true;
            Notify();
            // Čia galima inicijuoti Game objektą jei reikia
        }

        public void EndGame()
        {
            State = GameState.Finished;
            EndTime = DateTime.UtcNow;
            IsActive = false;
            Notify();
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