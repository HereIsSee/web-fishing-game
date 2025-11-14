namespace Api.Models.Facade
{
    using Api.Models;
    public interface IGameLogicSubsystem
    {
        bool TryFishCatch(Player player, Fish fish);
        void UpdatePlayerScore(Player player, int points);
        void ApplyDecoratorEffect(Player player, Decorator.IFishDecorator decorator);
        string GetGameLogicReport();
    }
}
