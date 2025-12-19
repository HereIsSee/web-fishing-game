namespace Api.Models
{
    public class SeaWaterGameEnvironment : GameEnvironment
    {
        private static readonly Random random = new Random();

        public SeaWaterGameEnvironment()
        {
            Width = 800;
            Height = 600;
            WaterLevelHeight = 500;
        }

        public SeaWaterGameEnvironment(int width, int height, int waterLevelHeight)
        {
            Width = width;
            Height = height;
            WaterLevelHeight = waterLevelHeight;
        }

        // Template step override
        protected override void MaintainPopulation()
        {
            if (Fishes.Count <= 15)
                RefillFishes();
        }

        private void RefillFishes()
        {
            FishAbstractFactory fishFactory = new SeaWaterFishFactory();
            int newFishCount = random.Next(5, 11);

            for (int i = 0; i < newFishCount; i++)
            {
                double x = random.NextDouble() * Width;
                double y = random.NextDouble() * WaterLevelHeight;

                int roll = random.Next(100);
                Fish fish = roll switch
                {
                    < 50 => fishFactory.CreateCommonFish(x, y),
                    < 80 => fishFactory.CreateRareFish(x, y),
                    < 90 => fishFactory.CreateLegendaryFish(x, y),
                    _ => fishFactory.CreateDangerFish(x, y)
                };

                Fishes.Add(fish);
            }
        }

        protected override bool EnableHazardZones() => true;
        protected override bool EnableDarkWater() => true;

        protected override List<HazardZone> GenerateHazardZones()
        {
            int count = random.Next(2, 5);
            var zones = new List<HazardZone>();
            for (int i = 0; i < count; i++)
            {
                zones.Add(new HazardZone
                {
                    X = (float)(random.NextDouble() * Width),
                    Y = (float)(random.NextDouble() * WaterLevelHeight),
                    Radius = random.Next(60, 140),
                    SpeedMultiplier = 0.5f
                });
            }
            return zones;
        }

        protected override DarkWaterSettings GenerateDarkWaterSettings()
        {
            return new DarkWaterSettings
            {
                Enabled = true,
                Alpha = 0.75f,
                VisibleRadius = 120f
            };
        }

        public override void DeleteFish(int fishId)
        {
            Fishes.RemoveAll(f => f.Id == fishId);
        }
    }
}
