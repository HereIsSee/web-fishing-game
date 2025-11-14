namespace Api.Models.Facade
{
    using Api.Models;
    public interface IGameRenderingSubsystem
    {
        void RenderFish(Fish fish);
        void RenderPlayer(Player player);
        void ClearScreen();
        string GetRenderReport();
    }
}
