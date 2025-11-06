namespace Api.Models
{
    public class BombFish : Fish
    {
        public BombFish(double x, double y) : base(x, y)
        {
            this.MovementSpeed = 0.0;
            this.Points = 15;
            this.Radius = 10.0;
            this.Color = "#880000ff";
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
