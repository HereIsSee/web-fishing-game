namespace Api.Models
{
    public abstract class FishAbstractFactory
    {
        public abstract Fish CreateCommonFish(double x, double y);
        public abstract Fish CreateRareFish(double x, double y);
        public abstract Fish CreateLegendaryFish(double x, double y);
    }
}
