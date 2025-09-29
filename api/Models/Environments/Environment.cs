namespace Api.Models
{
    public class Environment
    {
        public int Id { get; set; }

        // Environment has fishes
        public List<Fish> Fishes { get; set; }

        // Environment has obtacles
        public List<Obstacle> Obtacles { get; set; }
    }
}
