using Api.Models.Bridge;
using Api.Models.Flyweight;
using Api.Models.Decorator;

namespace Api.Models
{
    public class BlackFish : Fish
    {
        // NEW Flyweight constructor
        public BlackFish(FishSharedData sharedData, double x, double y) : base(sharedData, x, y)
        {
            this.Type = "BlackFish";
            this.Decorator = new NormalFishDecorator();
        }

        // OLD constructor
        public BlackFish(double x, double y) : base(x, y)
        {
            this.Type = "BlackFish";
            this.MovementSpeed = 2.0;
            this.Points = 15;
            this.Radius = 15.0;
            this.Color = "#000000";
            this.FishMove = new RandomMove();
            this.Behavior = new PassiveBehavior();
            this.Decorator = new NormalFishDecorator();
        }

        public override void UpdatePosition(int environmentWidth, int waterLevelHeight)
        {
            CurrentState.Update(this, environmentWidth, waterLevelHeight);
        }
    }
}