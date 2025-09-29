namespace Api.Models
{
    public class Obstacle
    {
        public int Id { get; set; }

        public double PositionX { get; set; }
        public double PositionY { get; set; }

        // Obstacle belongs to an environment
        public GameEnvironment? GameEnvironment { get; set; }
        public int GameEnvironmentId { get; set; }
        
    }
}
