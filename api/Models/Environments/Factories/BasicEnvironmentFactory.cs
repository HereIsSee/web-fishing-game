namespace Api.Models
{
    public class BasicEnvironmentFactory : GameEnvironmentFactory
    {
        public override GameEnvironment getEnvironment()
        {
            return new BasicGameEnvironment();
        }
    }
}
