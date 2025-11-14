namespace Api.Models.Facade
{
    using Api.Models;
    public class GameRenderingSubsystem : IGameRenderingSubsystem
    {
        private List<string> renderQueue = new();

        public void RenderFish(Fish fish)
        {
            renderQueue.Add($"Rendering {fish.GetType().Name} at position ({fish.PositionX}, {fish.PositionY})");
        }

        public void RenderPlayer(Player player)
        {
            renderQueue.Add($"Rendering Player {player.Name} with score {player.Score}");
        }

        public void ClearScreen()
        {
            renderQueue.Clear();
            renderQueue.Add("Screen cleared");
        }

        public string GetRenderReport()
        {
            return string.Join(" | ", renderQueue);
        }
    }
}
