namespace Api.Models
{
    public class SeatWaterFishFactory : FishAbstractFactory
    {
        public override Fish CreateCommonFish(double x, double y)
        {
            return new BlueFish(x, y);
        }
        public override Fish CreateRareFish(double x, double y)
        {
            return new BlackFish(x, y);
        }
        public override Fish CreateLegendaryFish(double x, double y)
        {
            return new YellowFish(x, y);
        }
    }
}
