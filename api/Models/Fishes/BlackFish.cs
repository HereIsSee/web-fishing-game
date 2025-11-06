namespace Api.Models
{
    public class BlackFish : Fish
    {
        public BlackFish(double x, double y) : base(x, y)
        {
            this.MovementSpeed = 2.0;
            this.Points = 15;
            this.Radius = 15.0;
            this.Color = "#000000";
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
