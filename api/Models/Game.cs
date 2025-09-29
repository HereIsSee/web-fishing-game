namespace Api.Models
{
    public class Game
    {
        public int Id { get; set; }

        // Game has an Environment
        public GameEnvironment? GameEnvironment { get; set; }
        public int GameEnvironmentId { get; set; }

        // Game has a Scoreboard
        public Scoreboard? Scoreboard { get; set; }
        public int ScoreboardId { get; set; }

        //Game has players
        public List<Player> Players { get; set; } = new();

        // Game belongs to a session 1 to 1
        public Session? Session { get; set; }
    }
}
