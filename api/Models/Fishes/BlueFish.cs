using Api.Models.Bridge;
using Api.Models.Flyweight;
using Api.Models.Decorator;  // ADD THIS LINE!

namespace Api.Models
{
    public class BlueFish : Fish
    {
        // NEW CONSTRUCTOR for Flyweight
        public BlueFish(FishSharedData sharedData, double x, double y) 
            : base(sharedData, x, y)
        {
            this.Type = "BlueFish";
            this.Decorator = new NormalFishDecorator();
        }

        // KEEP OLD constructor
        public BlueFish(double x, double y) : base(x, y)
        {
            this.Type = "BlueFish";
            this.MovementSpeed = 1.0;
            this.Points = 5;
            this.Radius = 10.0;
            this.Color = "#3A18B5";
            this.FishMove = new RandomMove();
            this.Behavior = new AggressiveBehavior();
            this.Decorator = new NormalFishDecorator();

            InitializeBaseSpeedIfNeeded();
            SetState(new IdleState());
        }

        public override void UpdatePosition(int environmentWidth, int waterLevelHeight)
        {
            CurrentState.Update(this, environmentWidth, waterLevelHeight);
        }
    }
}