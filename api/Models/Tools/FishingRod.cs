namespace Api.Models
{
    public class FishingRod : Tool
    {
        // should be double type x y coordinate
        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public bool HasHookedFish { get; set; }
        public bool Cast { get; set; }

        public FishingRod(double positionX, double positionY)
        {
            this.PositionX = positionX;
            this.PositionY = positionY;
            this.HasHookedFish = false;
            this.Cast = false;
        }
    }
}
