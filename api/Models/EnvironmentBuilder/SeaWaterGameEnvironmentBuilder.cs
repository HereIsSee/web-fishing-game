using Api.Models.Prototype;
using Api.Models.Decorator;
using Api.Models.Facade;
using System;

namespace Api.Models
{
    public class SeaWaterGameEnvironmentBuilder : IEnvironmentBuilder
    {
        private static readonly Random _random = new Random();
        public GameEnvironment _gameEnvironment = null!;
        private readonly GameFacade _gameFacade = new GameFacade();

        public IEnvironmentBuilder StartNew(GameEnvironment env)
        {
            _gameEnvironment = env;

            // Ensure FishGroups exists
            if (_gameEnvironment.FishGroups == null)
                _gameEnvironment.FishGroups = new List<IFishGroup>();

            return this;
        }

        public IEnvironmentBuilder AddFishes()
        {
            FishAbstractFactory fishFactory = new SeaWaterFishFactory();
            int fishCount = _random.Next(20, 36);

            // Prototypes (unchanged)
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

                if (roll < 50 && _random.Next(100) < 25)
                {
                    int schoolSize = _random.Next(4, 8);
                    double schoolRadius = _random.Next(30, 70);

                    var schoolFish = new List<Fish>();

                    for (int s = 0; s < schoolSize; s++)
                    {
                        // random point in circle around (x,y)
                        var angle = _random.NextDouble() * Math.PI * 2;
                        var r = Math.Sqrt(_random.NextDouble()) * schoolRadius;

                        var sx = x + Math.Cos(angle) * r;
                        var sy = y + Math.Sin(angle) * r;

                        // clamp inside water
                        sx = Math.Max(0, Math.Min(_gameEnvironment.Width, sx));
                        sy = Math.Max(0, Math.Min(_gameEnvironment.WaterLevelHeight, sy));

                        var fish = fishFactory.CreateCommonFish(sx, sy);

                        // Decorate each fish (same logic as single fish)
                        ApplyRandomDecorator(fish);

                        // Render frame (kept)
                        _gameFacade.RenderFrame(new Player("system", "Environment", sx, sy));

                        schoolFish.Add(fish);
                    }

                    _gameEnvironment.FishGroups.Add(new FishSchool(schoolFish));
                    continue;
                }

                // Otherwise spawn a single fish (leaf)
                Fish singleFish =
                    roll < 50 ? fishFactory.CreateCommonFish(x, y) :
                    roll < 80 ? fishFactory.CreateRareFish(x, y) :
                    roll < 90 ? fishFactory.CreateLegendaryFish(x, y) :
                                fishFactory.CreateDangerFish(x, y);

                // Prototype logging block (unchanged)
                if (i % 5 == 0)
                {
                    int prototypeType = i % 5;
                    FishPrototype clonedProto;

                    if (prototypeType == 0)
                        clonedProto = bluePrototype.CloneShallow();
                    else if (prototypeType == 1)
                        clonedProto = yellowPrototype.CloneDeep();
                    else if (prototypeType == 2)
                        clonedProto = blackPrototype.CloneShallow();
                    else if (prototypeType == 3)
                        clonedProto = bombPrototype.CloneDeep();
                    else
                        clonedProto = fatPrototype.CloneShallow();

                    Console.WriteLine($"Cloned prototype: {clonedProto.GetType().Name} at ({clonedProto.PositionX}, {clonedProto.PositionY})");
                }

                // Decorator logic (unchanged)
                ApplyRandomDecorator(singleFish);

                // Render frame (kept)
                _gameFacade.RenderFrame(new Player("system", "Environment", x, y));

                // Composite insert
                _gameEnvironment.FishGroups.Add(new FishLeaf(singleFish));
            }

            return this;
        }

        private void ApplyRandomDecorator(Fish fish)
        {
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
