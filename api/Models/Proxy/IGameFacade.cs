namespace Api.Models.Proxy
{
    using Api.Models;
    using Api.Models.Decorator;

    /// <summary>
    /// Interface for GameFacade - enables Proxy pattern
    /// </summary>
    public interface IGameFacade
    {
        void InitializeGame();
        bool AttemptFishCatch(Player player, Fish fish);
        void ApplyEffect(Player player, IFishDecorator decorator);
        void RenderFrame(Player player);
        void RenderAllPlayers(List<Player> players);
        void UpdateAllPlayerScores(List<Player> players);
        void PlayGameOverSound();
        void PlaySuccessSound();
        void UpdateGameState();
    }
}
