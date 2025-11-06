namespace Api.Models
{
    public interface IEnvironmentBuilder
    {   
        public IEnvironmentBuilder StartNew(GameEnvironment env);

        public IEnvironmentBuilder AddFishes();

        public IEnvironmentBuilder AddObstacles();

        public GameEnvironment Build();
    }
}
