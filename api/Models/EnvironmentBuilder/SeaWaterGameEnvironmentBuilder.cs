namespace Api.Models
{
    public class SeaWaterGameEnvironmentBuilder : IEnvironmentBuilder
    {
        private static readonly Random _random = new Random();
        public GameEnvironment _gameEnvironment = null!;
        public IEnvironmentBuilder StartNew(GameEnvironment env)
        {
            _gameEnvironment = env;
            return this;
        }

        public IEnvironmentBuilder AddFishes()
        {
            FishAbstractFactory fishFactory = new SeatWaterFishFactory();
            int fishCount = _random.Next(20, 36);
            

            for (int i = 0; i < fishCount; i++)
            {
                double x = _random.NextDouble() * _gameEnvironment.Width;
                double y = _random.NextDouble() * _gameEnvironment.WaterLevelHeight;

                int roll = _random.Next(100);

                Fish fish;
                if (roll < 60)
                    fish = fishFactory.CreateCommonFish(x, y);
                else if (roll < 90)
                    fish = fishFactory.CreateRareFish(x, y);
                else
                    fish = fishFactory.CreateLegendaryFish(x, y);

                _gameEnvironment.Fishes.Add(fish);
            }

            return this;

        }

        public IEnvironmentBuilder AddObstacles()
        {
            return this;
        }

        public GameEnvironment Build()
        {
            return _gameEnvironment;
        }
    }
}
