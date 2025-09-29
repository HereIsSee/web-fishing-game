namespace Api.Models
{
    public class FishingRod : Tool
    {
        // should be double type x y coordinate
        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public boolean HasHookedFish { get; set; } 
    }
}
