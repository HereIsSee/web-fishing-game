using Api.Models.Bridge;
using Api.Models.Flyweight;
using Api.Models.Decorator;

namespace Api.Models
{
    public class YellowFish : Fish
    {
        // NEW Flyweight constructor
        public YellowFish(FishSharedData sharedData, double x, double y) : base(sharedData, x, y)
        {
            this.Type = "YellowFish";
            this.Decorator = new NormalFishDecorator();
        }

        // OLD constructor
        public YellowFish(double x, double y) : base(x, y)
        {
            this.Type = "YellowFish";
            this.MovementSpeed = 5.0;
            this.Points = 30;
            this.Radius = 5.0;
            this.Color = "#D9E65A";
            this.FishMove = new RandomMove();
            this.Behavior = new TerritorialBehavior();
            this.Decorator = new NormalFishDecorator();
        }

        public override void UpdatePosition(int environmentWidth, int waterLevelHeight)
        {
            CurrentState.Update(this, environmentWidth, waterLevelHeight);
        }
    }
}