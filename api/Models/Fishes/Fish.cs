namespace Api.Models
{
    public enum FishType
    {
        BasicFish,
        RedFish,
        GoldenFish,
        BombFish
    }
    public class Fish
    {
        private static int _nextId = 1; 
        public int Id { get; set; }

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public double MovementSpeed { get; set; } = 10.0;

        public bool HasBeenHooked { get; set; } = false;

        public int Points { get; set; }
        public FishType Type { get; set; }

        private double DirectionX { get; set; }
        private double DirectionY { get; set; }
        private int FramesUntilDirectionChange { get; set; }
        private static readonly Random random = new Random();

        public Fish(FishType type, double positionX, double positionY)
        {
            this.Type = type;
            Id = _nextId++; 
            
            this.HasBeenHooked = false;
            this.PositionX = positionX;
            this.PositionY = positionY;

            switch (type)
            {
                case FishType.BasicFish:
                    MovementSpeed = 5.0;
                    Points = 10;
                    break;
                case FishType.RedFish:
                    MovementSpeed = 10.0;
                    Points = 20;
                    break;
                case FishType.GoldenFish:
                    MovementSpeed = 15.0;
                    Points = 50;
                    break;
                case FishType.BombFish:
                    MovementSpeed = 0.0;
                    Points = 50;
                    break;
                default:
                    MovementSpeed = 2.0;
                    Points = 5;
                    break;
            }

        }
        
        private void RandomizeDirection()
        {
            // Random direction on a unit circle
            double angle = random.NextDouble() * Math.PI * 2;
            DirectionX = Math.Cos(angle);
            DirectionY = Math.Sin(angle) * 0.3; // smaller vertical range

            // Random time until next change (e.g., 1–3 seconds at 10 updates/s)
            FramesUntilDirectionChange = random.Next(10, 30);
        }

        public void UpdatePosition(double width, double waterLevelHeight)
        {
            if (HasBeenHooked) return;

            // Move in the current direction
            PositionX += DirectionX * MovementSpeed * 0.1;
            PositionY += DirectionY * MovementSpeed * 0.1;

            // Occasionally change direction
            FramesUntilDirectionChange--;
            if (FramesUntilDirectionChange <= 0)
            {
                RandomizeDirection();
            }

            // Bounce off edges
            if (PositionX < 0)
            {
                PositionX = 0;
                DirectionX = Math.Abs(DirectionX);
            }
            else if (PositionX > width)
            {
                PositionX = width;
                DirectionX = -Math.Abs(DirectionX);
            }

            if (PositionY < 0)
            {
                PositionY = 0;
                DirectionY = Math.Abs(DirectionY);
            }
            else if (PositionY > waterLevelHeight)
            {
                PositionY = waterLevelHeight;
                DirectionY = -Math.Abs(DirectionY);
            }
        }


    }
}
