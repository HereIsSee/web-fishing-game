namespace Api.Models
{
    public class BlueFish : Fish
    {
        public BlueFish(double x, double y) : base(x, y)
        {
            this.MovementSpeed = 1.0;
            this.Points = 5;
            this.Radius = 10.0;
            this.Color = "#3A18B5";
            this.FishMove = new RandomMove();

        }
        public override void UpdatePosition(int environmentWidth, int waterLevelHeight)
        {
            this.FishMove.Move(this, environmentWidth, waterLevelHeight);

            if(IsTouchingBoundary(environmentWidth, waterLevelHeight))
            {
                this.FishMove = GetNewMovementStrategy();
            }
        }

    }
}
