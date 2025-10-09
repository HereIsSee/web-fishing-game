namespace Api.Models
{

    public enum ObstacleType {
        Seaweed,
        Rock
        
    }
    public class Obstacle
    {
        public int Id { get; set; }

        public double PositionX { get; set; }
        public double PositionY { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int ResistencePercentage { get; set; }

        public ObstacleType ObstacleType { get; set; } = ObstacleType.Seaweed;
    }
}
