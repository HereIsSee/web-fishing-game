namespace Api.Models.Facade
{
    using Api.Models;
    public class GameLogicSubsystem : IGameLogicSubsystem
    {
        private List<string> logicLog = new();

        public bool TryFishCatch(Player player, Fish fish)
        {
            double escapeChance = fish.Behavior?.GetEscapeProbability() ?? 0.5;
            bool caught = new Random().NextDouble() > escapeChance;
            
            string result = caught 
                ? $"Player {player.Name} caught {fish.GetType().Name} for {fish.Points} points"
                : $"Player {player.Name} missed the {fish.GetType().Name}";
            
            logicLog.Add(result);
            return caught;
        }

        public void UpdatePlayerScore(Player player, int points)
        {
            player.Score += points;
            logicLog.Add($"Score updated: {player.Name} now has {player.Score} points");
        }

        public void ApplyDecoratorEffect(Player player, Decorator.IFishDecorator decorator)
        {
            logicLog.Add($"Applied decorator effect to player {player.Name}");
        }

        public string GetGameLogicReport()
        {
            return string.Join(" | ", logicLog);
        }
    }
}
