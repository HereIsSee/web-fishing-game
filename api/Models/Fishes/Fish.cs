namespace Api.Models
{
    public abstract class Fish
    {
        private static int _nextId = 1;
        private static readonly object _idLock = new();
        public int Id { get; set; }

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public double MovementSpeed { get; set; }

        public bool HasBeenHooked { get; set; } = false;

        public int Points { get; set; }
        public double Radius { get; set; }
        public string Color { get; set; } = null!;
        public IFishMove FishMove { get; set; } = null!;

        protected Fish(double x, double y)
        {
            lock (_idLock)
            {
                Id = _nextId++;
            }

            PositionX = x;
            PositionY = y;
        }

        public abstract void UpdatePosition(int environmentWidth, int waterLevelHeight);

        protected bool IsTouchingBoundary(int environmentWidth, int waterLevelHeight)
        {
            return PositionX <= 0 ||
                PositionX >= environmentWidth ||
                PositionY <= 0 ||
                PositionY >= waterLevelHeight;
        }
        protected IFishMove GetNewMovementStrategy()
        {
            Random random = new Random();
            int move = random.Next(5);

            return move switch
            {
                0 => new LeftMove(),
                1 => new RightMove(),
                2 => new UpMove(),
                3 => new DownMove(),
                4 => new RandomMove(),
                _ => new RandomMove()
            };
        }

        // private static int _nextId = 1; 
        // private double DirectionX { get; set; }
        // private double DirectionY { get; set; }
        // private int FramesUntilDirectionChange { get; set; }
        // private static readonly Random random = new Random();

        // public Fish(FishType type, double positionX, double positionY)
        // {
        //     Id = _nextId++; 
            
        //     this.HasBeenHooked = false;
        //     this.PositionX = positionX;
        //     this.PositionY = positionY;

        //     switch (type)
        //     {
        //         case FishType.BasicFish:
        //             MovementSpeed = 5.0;
        //             Points = 10;
        //             break;
        //         case FishType.RedFish:
        //             MovementSpeed = 10.0;
        //             Points = 20;
        //             break;
        //         case FishType.GoldenFish:
        //             MovementSpeed = 15.0;
        //             Points = 50;
        //             break;
        //         default:
        //             MovementSpeed = 2.0;
        //             Points = 5;
        //             break;
        //     }

        // }
        
        // private void RandomizeDirection()
        // {
        //     // Random direction on a unit circle
        //     double angle = random.NextDouble() * Math.PI * 2;
        //     DirectionX = Math.Cos(angle);
        //     DirectionY = Math.Sin(angle) * 0.3; // smaller vertical range

        //     // Random time until next change (e.g., 1–3 seconds at 10 updates/s)
        //     FramesUntilDirectionChange = random.Next(10, 30);
        // }

        // public void UpdatePosition(double width, double waterLevelHeight)
        // {
        //     if (HasBeenHooked) return;

        //     // Move in the current direction
        //     PositionX += DirectionX * MovementSpeed * 0.1;
        //     PositionY += DirectionY * MovementSpeed * 0.1;

        //     // Occasionally change direction
        //     FramesUntilDirectionChange--;
        //     if (FramesUntilDirectionChange <= 0)
        //     {
        //         RandomizeDirection();
        //     }

        //     // Bounce off edges
        //     if (PositionX < 0)
        //     {
        //         PositionX = 0;
        //         DirectionX = Math.Abs(DirectionX);
        //     }
        //     else if (PositionX > width)
        //     {
        //         PositionX = width;
        //         DirectionX = -Math.Abs(DirectionX);
        //     }

        //     if (PositionY < 0)
        //     {
        //         PositionY = 0;
        //         DirectionY = Math.Abs(DirectionY);
        //     }
        //     else if (PositionY > waterLevelHeight)
        //     {
        //         PositionY = waterLevelHeight;
        //         DirectionY = -Math.Abs(DirectionY);
        //     }
        // }
    }
}
