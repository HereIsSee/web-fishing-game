namespace Api.Models
{
    using Api.Models.Bridge;
    using Api.Models.Flyweight;
    using Api.Models.Decorator;

    public abstract class Fish
    {
        private static int _nextId = 1;
        private static readonly object _idLock = new();
        
        // FLYWEIGHT: Shared data field (NEW)
        private FishSharedData _sharedData = null;
        
        public int Id { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public bool HasBeenHooked { get; set; } = false;

        // OLD properties (keep for backward compatibility)
        public string Type { get; protected set; } = string.Empty;

        public double MovementSpeed { get; set; }
        public int Points { get; set; }
        public double Radius { get; set; }
        public string Color { get; set; } = null!;
        public IFishMove FishMove { get; set; } = null!;
        public IFishBehavior Behavior { get; set; }
        public IFishDecorator Decorator { get; set; } = null!;

        // NEW CONSTRUCTOR for Flyweight (ADD THIS)
        protected Fish(FishSharedData sharedData, double x, double y)
        {
            lock (_idLock)
            {
                Id = _nextId++;
            }

            _sharedData = sharedData;
            PositionX = x;
            PositionY = y;
            Decorator = new NormalFishDecorator();
            
            // Initialize OLD properties from shared data
            MovementSpeed = _sharedData.BaseSpeed;
            Points = (int)_sharedData.BasePoints;
            Radius = _sharedData.BaseRadius;
            Color = _sharedData.BaseColor;
            FishMove = _sharedData.MovementPattern;
            Behavior = _sharedData.Behavior;
        }

        // OLD CONSTRUCTOR (KEEP THIS)
        protected Fish(double x, double y)
        {
            lock (_idLock)
            {
                Id = _nextId++;
            }

            PositionX = x;
            PositionY = y;
            // Old constructor doesn't set other properties - 
            // child classes will set them individually
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

        // NEW PROPERTY: Get Points with decorator multiplier (Flyweight compatible)
        public int GetDecoratedPoints()
        {
            if (_sharedData != null)
                return (int)(_sharedData.BasePoints * Decorator.GetPointsMultiplier());
            else
                return (int)(Points * Decorator.GetPointsMultiplier());
        }

        // NEW METHOD: Check if using Flyweight
        public bool IsUsingFlyweight() => _sharedData != null;
    }
}