using Api.Models.Bridge;
using Api.Models.Flyweight;
using Api.Models.Decorator;

namespace Api.Models
{
    public class BombFish : Fish
    {
        // NEW Flyweight constructor
        public BombFish(FishSharedData sharedData, double x, double y) : base(sharedData, x, y)
        {
            this.Decorator = new NormalFishDecorator();
        }

        // OLD constructor
        public BombFish(double x, double y) : base(x, y)
        {
            this.MovementSpeed = 0.0;
            this.Points = 15;
            this.Radius = 10.0;
            this.Color = "#880000ff";
            this.FishMove = new RandomMove();
            this.Behavior = new NocturnalBehavior();
            this.Decorator = new NormalFishDecorator();
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