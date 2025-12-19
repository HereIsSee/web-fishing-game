namespace Api.Models
{
    using Api.Models.Facade;
    using Api.Models.Proxy;

    public class Scoreboard : Observer, IScoreboard
    {
        public int Id { get; set; }
        public Dictionary<string, int> PlayerScores { get; set; }
        public string CurrentGameState { get; set; } = "Waiting";
        public int RemainingTime { get; set; }
        private readonly GameFacade _gameFacade = new GameFacade();

        public Scoreboard()
        {
            PlayerScores = new Dictionary<string, int>();
        }

        public override void Update(Session session)
        {
            UpdateScores(session);
        }

        // IScoreboard implementation
        public void UpdateScores(Session session)
        {
            PlayerScores.Clear();
            foreach (var player in session.Players.Values)
            {
                Console.WriteLine($"🔴 Player {player.Name} Score: {player.Score}");
                PlayerScores[player.Name] = player.Score;
                _gameFacade.PlaySuccessSound();
            }
            CurrentGameState = session.State.ToString();
            RemainingTime = session.TimerDuration;
        }

        public void ResetScore(string playerName, string requesterId)
        {
            // Direct implementation - no security check
            if (PlayerScores.ContainsKey(playerName))
            {
                PlayerScores[playerName] = 0;
            }
        }

        public void AddPoints(string playerName, int points, string requesterId)
        {
            // Direct implementation - no security check
            if (PlayerScores.ContainsKey(playerName))
            {
                PlayerScores[playerName] += points;
            }
            else
            {
                PlayerScores[playerName] = points;
            }
        }

        public void ResetAllScores(string requesterId)
        {
            // Direct implementation - no security check
            PlayerScores.Clear();
        }
    }
}