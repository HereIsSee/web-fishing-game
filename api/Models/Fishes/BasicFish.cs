namespace Api.Models
{
    public class BasicFish : Fish
    {
        public double PositionX { get; set; }
    
        public double PositionY { get; set; }

        public double MovementSpeed { get; set; } = 10.0;

        public boolean HasBeenHooked { get; set; } = false;
        
    }
}
