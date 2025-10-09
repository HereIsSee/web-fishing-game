namespace Api.Models
{
    public class Game
    {
        public Game(int width, int height, int waterLevelHeight, int numberOfFish, int numberOfObstacles)
        {
            this.GameEnvironment = new GameEnvironment(width, height, waterLevelHeight, numberOfFish, numberOfObstacles);
            this.Scoreboard = new Scoreboard();   
        }
        public int Id { get; set; }
        public GameEnvironment GameEnvironment { get; set; }
        public Scoreboard Scoreboard { get; set; }

    }
}
