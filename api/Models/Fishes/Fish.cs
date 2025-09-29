namespace Api.Models
{
    public class Fish
    {
        public int Id { get; set; }

        // Fish belongs to an environment
        public GameEnvironment? GameEnvironment { get; set; }
        public int GameEnvironmentId { get; set; }

    }
}
