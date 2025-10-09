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

        public Obstacle(ObstacleType type, double positionX, double positionY)
        {
            this.PositionX = positionX;
            this.PositionY = positionY;

            switch (type)
            {
                case ObstacleType.Seaweed:
                    this.ResistencePercentage = 30;
                    this.Width = 30;
                    this.Height = 70;
                    break;
                case ObstacleType.Rock:
                    this.ResistencePercentage = 100;
                    this.Width = 20;
                    this.Height = 20;
                    break;
                default:
                    break;
            }
        }
    }
}
