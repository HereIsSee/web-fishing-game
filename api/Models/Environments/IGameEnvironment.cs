namespace Api.Models
{
    public abstract class GameEnvironment
    {
        public int Id { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int WaterLevelHeight { get; set; }
        public List<Fish> Fishes { get; set; } = new();
        public List<Obstacle> Obstacles { get; set; } = new();
        
        public abstract void GenerateRandomFishes(int count);

        public abstract void GenerateRandomObstacles(int count);

        public abstract void Update();

        public abstract void DeleteFish(int fishId);
    }
}
