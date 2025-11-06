namespace Api.Models
{
    public class Scoreboard : Observer
    {
        public int Id { get; set; }
        public Dictionary<string, int> PlayerScores { get; set; }
        public string CurrentGameState { get; set; }
        public int RemainingTime { get; set; }

        public Scoreboard()
        {
            PlayerScores = new Dictionary<string, int>();
        }

        public override void Update(Session session)
        {
            PlayerScores.Clear();
            foreach (var player in session.Players.Values)
            {
                Console.WriteLine($"🔴 Player {player.Name} Score: {player.Score}");
                PlayerScores[player.Name] = player.Score;
            }
            CurrentGameState = session.State.ToString();
            RemainingTime = session.TimerDuration;
        }
    }
}