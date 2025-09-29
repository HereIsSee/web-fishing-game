namespace Api.Models
{
    public class Obstacle
    {
        public int Id { get; set; }

        public double PositionX { get; set; }
        public double PositionY { get; set; }

        // Obstacle belongs to an environment
        public Environment? Environment { get; set; }
        public int EnvironmentId { get; set; }
        
    }
}
