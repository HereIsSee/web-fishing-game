namespace Api.Models
{
    public class Fish
    {
        public int Id { get; set; }

        // Fish belongs to an environment
        public Environment? Environment { get; set; }
        public int EnvironmentId { get; set; }
        
    }
}
