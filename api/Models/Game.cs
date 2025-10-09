namespace Api.Models
{
    public class Game
    {
        public int Id { get; set; }
        public GameEnvironment GameEnvironment { get; set; }
        public Scoreboard Scoreboard { get; set; }

    }
}
