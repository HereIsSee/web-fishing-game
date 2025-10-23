namespace Api.Models
{
    public class Game
    {
        public Game(int width, int height, int waterLevelHeight, int numberOfFish, int numberOfObstacles)
        {
            this.GameEnvironment = new GameEnvironment(width, height, waterLevelHeight, numberOfFish, numberOfObstacles);
        }
        public int Id { get; set; }
        public GameEnvironment GameEnvironment { get; set; }

    }
}
