using Api.Models.Flyweight;  // ADD THIS!

namespace Api.Models
{
    public class SeaWaterFishFactory : FishAbstractFactory  // Fixed typo: SeatWater → SeaWater
    {
        public override Fish CreateCommonFish(double x, double y)
        {
            var sharedData = FishFlyweightFactory.GetFlyweight("BlueFish");
            return new BlueFish(sharedData, x, y);
        }
        
        public override Fish CreateRareFish(double x, double y)
        {
            var sharedData = FishFlyweightFactory.GetFlyweight("BlackFish");
            return new BlackFish(sharedData, x, y);  // Add sharedData parameter
        }
        
        public override Fish CreateLegendaryFish(double x, double y)
        {
            var sharedData = FishFlyweightFactory.GetFlyweight("YellowFish"); // Need to add YellowFish to factory
            return new YellowFish(sharedData, x, y);  // Add sharedData parameter
        }
        
        public override Fish CreateDangerFish(double x, double y)
        {
            var sharedData = FishFlyweightFactory.GetFlyweight("BombFish");
            return new BombFish(sharedData, x, y);  // Add sharedData parameter
        }
    }
}