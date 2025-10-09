namespace Api.Models
{
    public enum FishType
    {
        BasicFish,
        RedFish,
        GoldenFish //Can add other fish types
    }
    public class Fish
    {
        public int Id { get; set; }

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public double MovementSpeed { get; set; } = 10.0;

        public bool HasBeenHooked { get; set; } = false;

        public Fish(FishType type, double positionX, double positionY)
        {
            this.HasBeenHooked = false;
            this.PositionX = positionX;
            this.PositionY = positionY;

            switch (type)
            {
                case FishType.BasicFish:
                    MovementSpeed = 5.0;
                    break;
                case FishType.RedFish:
                    MovementSpeed = 10.0;
                    break;
                case FishType.GoldenFish:
                    MovementSpeed = 15.0;
                    break;
                default:
                    MovementSpeed = 2.0;
                    break;
            }
            
        }


    }
}
