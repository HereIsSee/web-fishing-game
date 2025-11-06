namespace Api.Models
{
    public class YellowFish : Fish
    {
        public YellowFish(double x, double y) : base(x, y)
        {
            this.MovementSpeed = 5.0;
            this.Points = 30;
            this.Radius = 5.0;
            this.Color = "#D9E65A";
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
