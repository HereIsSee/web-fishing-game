namespace Api.Models
{
    using Api.Models.Bridge;
    public class FatFish : Fish
    {
        public FatFish(double x, double y) : base(x, y)
        {
            this.MovementSpeed = 3.0;
            this.Points = 50;
            this.Radius = 30.0;
            this.Color = "#571d72ff";
            this.FishMove = new RandomMove();
            this.Behavior = new TerritorialBehavior();
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
