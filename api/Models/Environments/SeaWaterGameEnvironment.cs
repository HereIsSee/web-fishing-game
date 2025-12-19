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
            var totalFish = GetAllFishesFlat().Count;
            if (totalFish <= 15)
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

                // COMMON fish: sometimes spawn as a school
                if (roll < 50)
                {
                    bool spawnSchool = random.Next(100) < 25; // 25% chance
                    if (spawnSchool)
                    {
                        // school size + spawn radius
                        int schoolSize = random.Next(4, 8);
                        double schoolRadius = random.Next(30, 70);

                        var fishes = new List<Fish>();
                        for (int s = 0; s < schoolSize; s++)
                        {
                            // random point in a circle around (x,y)
                            var angle = random.NextDouble() * Math.PI * 2;
                            var r = Math.Sqrt(random.NextDouble()) * schoolRadius;

                            var sx = x + Math.Cos(angle) * r;
                            var sy = y + Math.Sin(angle) * r;

                            // clamp inside water
                            sx = Math.Max(0, Math.Min(Width, sx));
                            sy = Math.Max(0, Math.Min(WaterLevelHeight, sy));

                            fishes.Add(fishFactory.CreateCommonFish(sx, sy));
                        }

                        FishGroups.Add(new FishSchool(fishes));
                        continue; // IMPORTANT: we already added the group
                    }

                    // otherwise single common fish
                    var fish = fishFactory.CreateCommonFish(x, y);
                    FishGroups.Add(new FishLeaf(fish));
                    continue;
                }

                // Non-common fish: spawn as single fish
                Fish single = roll switch
                {
                    < 80 => fishFactory.CreateRareFish(x, y),
                    < 90 => fishFactory.CreateLegendaryFish(x, y),
                    _ => fishFactory.CreateDangerFish(x, y)
                };

                FishGroups.Add(new FishLeaf(single));
            }
        }

        protected override bool EnableHazardZones() => true;
        protected override bool EnableDarkWater() => false;

        protected override List<HazardZone> GenerateHazardZones()
        {
            int count = random.Next(2, 5);
            var zones = new List<HazardZone>();

            for (int i = 0; i < count; i++)
            {
                int radius = random.Next(60, 140);

                // If the water area is too small for this radius, clamp radius down.
                int maxRadiusX = Math.Max(1, Width / 2);
                int maxRadiusY = Math.Max(1, WaterLevelHeight / 2);
                int maxAllowedRadius = Math.Min(maxRadiusX, maxRadiusY);

                if (radius > maxAllowedRadius)
                    radius = maxAllowedRadius;

                float minX = radius;
                float maxX = Width - radius;
                float minY = radius;
                float maxY = WaterLevelHeight - radius;

                // If something is degenerate, fall back to safe values
                float x = (maxX >= minX)
                    ? (float)(minX + random.NextDouble() * (maxX - minX))
                    : Width / 2f;

                float y = (maxY >= minY)
                    ? (float)(minY + random.NextDouble() * (maxY - minY))
                    : WaterLevelHeight / 2f;

                zones.Add(new HazardZone
                {
                    X = x,
                    Y = y,
                    Radius = radius,
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
            // Remove fish from whatever group it belongs to
            for (int i = FishGroups.Count - 1; i >= 0; i--)
            {
                var group = FishGroups[i];
                var removed = group.RemoveFish(fishId);

                // if this was a school and it became empty, remove the group
                if (group is FishSchool school && school.IsEmpty)
                {
                    FishGroups.RemoveAt(i);
                    continue;
                }

                // if this was a leaf and it now contains null, remove it
                if (group is FishLeaf leaf && !leaf.Flatten().Any())
                {
                    FishGroups.RemoveAt(i);
                    continue;
                }

                // Once removed, we're done
                if (removed) break;
            }
        }

    }
}
