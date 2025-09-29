namespace Api.Models
{
    public class GameEnvironment
    {
        public int Id { get; set; }

        // Environment has fishes
        public List<Fish> Fishes { get; set; } = new();

        // Environment has obtacles
        public List<Obstacle> Obstacles { get; set; } = new();
    }
}
