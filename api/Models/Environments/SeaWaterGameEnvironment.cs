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

        public override void Update()
        {
            foreach (var fish in Fishes)
            {
                fish.UpdatePosition(Width, WaterLevelHeight);
            }

            if (Fishes.Count <= 15)
            {
                RefillFishes();
            }
        }
        private void RefillFishes()
        {
            FishAbstractFactory fishFactory = new SeatWaterFishFactory();
            int newFishCount = random.Next(5, 11);


            for (int i = 0; i < newFishCount; i++)
            {
                double x = random.NextDouble() * Width;
                double y = random.NextDouble() * WaterLevelHeight;

                int roll = random.Next(100);
                Fish fish = roll switch
                {
                    < 60 => fishFactory.CreateCommonFish(x, y),
                    < 90 => fishFactory.CreateRareFish(x, y),
                    _ => fishFactory.CreateLegendaryFish(x, y)
                };

                Fishes.Add(fish);
            }
        }

        public override void DeleteFish(int fishId)
        {
            Fishes.RemoveAll(f => f.Id == fishId);
        }
    }
}
