namespace Api.Models
{
    public class SeaWaterEnvironmentFactory : GameEnvironmentFactory
    {
        public override GameEnvironment getEnvironment()
        {
            IEnvironmentBuilder builder = new SeaWaterGameEnvironmentBuilder();

            GameEnvironment environment = builder
                .StartNew(new SeaWaterGameEnvironment())
                .AddFishes()
                .AddObstacles()
                .Build();

            return environment;
        }
    }
}
