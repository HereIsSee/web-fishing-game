namespace Api.Models
{
    public class GameEnvironment
    {
        public int Id { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int WaterLevelHeight { get; set; }

        // Environment has fishes
        public List<Fish> Fishes { get; set; } = new();

        // Environment has obtacles
        public List<Obstacle> Obstacles { get; set; } = new();

        private static readonly Random random = new Random();

        public GameEnvironment(int width, int height, int waterLevelHeight,
            int numberOfFish, int numberOfObstacles)
        {
            Width = width;
            Height = height;
            WaterLevelHeight = waterLevelHeight;

            GenerateRandomFishes(numberOfFish);
            GenerateRandomObstacles(numberOfObstacles);
        }

        private void GenerateRandomFishes(int count)
        {
            Array fishTypes = Enum.GetValues(typeof(FishType));

            for (int i = 0; i < count; i++)
            {
                FishType type = (FishType)fishTypes.GetValue(random.Next(fishTypes.Length));
                double posX = random.NextDouble() * Width;
                double posY = random.NextDouble() * WaterLevelHeight; // fish stay in water
                Fishes.Add(new Fish(type, posX, posY));
            }
        }

        private void GenerateRandomObstacles(int count)
        {
            Array obstacleTypes = Enum.GetValues(typeof(ObstacleType));

            for (int i = 0; i < count; i++)
            {
                ObstacleType type = (ObstacleType)obstacleTypes.GetValue(random.Next(obstacleTypes.Length));
                double posX = random.NextDouble() * Width;
                double posY = random.NextDouble() * WaterLevelHeight; // obstacles in water too
                Obstacles.Add(new Obstacle(type, posX, posY));
            }
        }

        public void Update()
        {
            foreach (var fish in Fishes)
            {
                fish.UpdatePosition(Width, WaterLevelHeight);
            }

             if (Fishes.Count <= 20)
            {
                int newFishCount = random.Next(5, 16);
                GenerateRandomFishes(newFishCount);
            }
        }

        public void DeleteFish(int fishId)
        {
            Fishes.RemoveAll(f => f.Id == fishId);
        }
    }
}
