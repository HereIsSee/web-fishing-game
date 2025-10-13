namespace Api.Models
{
    public class Boat
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double MovementSpeed { get; set; }

        public Boat(double positionX, double positionY)
        {
            this.Width = 20;
            this.Height = 10;
            this.PositionX = positionX;
            this.PositionY = positionY;
            this.MovementSpeed = 10.0;
        }

        public void setPositionX(double x)
        {
            this.PositionX = x;
        }
    }
}
