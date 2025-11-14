using Api.Models.Prototype;
using Api.Models.Decorator;

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
            FishPrototype bluePrototype = new BlueFishShallow();
            FishPrototype yellowPrototype = new YellowFishShallow();
            FishPrototype blackPrototype = new BlackFishShallow();
            FishPrototype bombPrototype = new BombFishShallow();
            FishPrototype fatPrototype = new FatFishShallow();

            for (int i = 0; i < fishCount; i++)
            {
                double x = _random.NextDouble() * _gameEnvironment.Width;
                double y = _random.NextDouble() * _gameEnvironment.WaterLevelHeight;

                int roll = _random.Next(100);
                Fish fish;
                if (roll < 50)
                {
                    var template = fishFactory.CreateCommonFish(x, y);
                    fish = template;
                }
                else if (roll < 80)
                {
                    var template = fishFactory.CreateRareFish(x, y);
                    fish = template;
                }
                else if (roll < 90)
                {
                    var template = fishFactory.CreateLegendaryFish(x, y);
                    fish = template;
                }
                else
                {
                    var template = fishFactory.CreateDangerFish(x, y);
                    fish = template;
                }

                int decoratorRoll = _random.Next(100);
                if (decoratorRoll < 80)
                {
                    fish.Decorator = new NormalFishDecorator();
                }
                else if (decoratorRoll < 90)
                {
                    fish.Decorator = new WeightedFishDecorator();
                }
                else
                {
                    fish.Decorator = new PoisonedFishDecorator();
                }

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
