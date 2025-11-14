using Api.Models.Decorator;

namespace Api.Models.Fishes
{
    public class DecoratedFish
    {
        public Fish BaseFish { get; private set; }
        public IFishDecorator Decorator { get; private set; }
        public DecoratedFish(Fish fish, IFishDecorator decorator)
        {
            BaseFish = fish;
            Decorator = decorator;
        }
        public int GetDecoratedPoints()
        {
            int basePoints = BaseFish.Points;
            double multiplier = Decorator.GetPointsMultiplier();
            return (int)(basePoints * multiplier);
        }
        public bool WillCauseFreeze()
        {
            return Decorator.CausesFreeze();
        }
        public string GetDecoratorSummary()
        {
            return $"{BaseFish.Color} Fish ({Decorator.DecoratorType}) - Base Points: {BaseFish.Points}, Decorated Points: {GetDecoratedPoints()}";
        }
    }
}
