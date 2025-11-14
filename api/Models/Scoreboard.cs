namespace Api.Models
{
    using Api.Models.Facade;

    public class Scoreboard : Observer
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
    }
}