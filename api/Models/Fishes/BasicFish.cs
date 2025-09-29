namespace Api.Models
{
    public class BasicFish : Fish
    {
        public double PositionX { get; set; }
    
        public double PositionY { get; set; }

        public double MovementSpeed { get; set; } = 10.0;

        public bool HasBeenHooked { get; set; } = false;

    }
}
