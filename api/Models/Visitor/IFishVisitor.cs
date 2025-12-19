namespace Api.Models.Visitor
{
    using Api.Models.Fishes;

    /// <summary>
    /// VISITOR PATTERN - Visitor Interface
    /// Allows adding new operations to Fish objects without modifying their classes
    /// </summary>
    public interface IFishVisitor
    {
        void Visit(BlackFish fish);
        void Visit(BlueFish fish);
        void Visit(YellowFish fish);
        void Visit(BombFish fish);
        void Visit(FatFish fish);
    }
}
