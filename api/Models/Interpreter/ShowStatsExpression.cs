namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Terminal Expression
    /// Command: "show stats" - Displays current game statistics
    /// </summary>
    public class ShowStatsExpression : IExpression
    {
        public void Interpret(GameAdminContext context)
        {
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            context.Log("Executing: SHOW STATS");
            context.Log("");
            context.Log("📊 GAME STATISTICS");
            context.Log("═══════════════════════════════════════════");
            
            context.Log($"🎮 Session Status: {(context.Session.IsActive ? "ACTIVE" : "INACTIVE")}");
            context.Log($"🎯 Game State: {context.Session.State}");
            context.Log($"👥 Total Players: {context.Session.Players.Count}");
            context.Log($"🐟 Total Fish: {context.Session.Environment.GetAllFishesFlat().Count}");
            context.Log($"🚧 Total Obstacles: {context.Session.Environment.Obstacles.Count}");
            
            if (context.Session.Players.Any())
            {
                context.Log("\n📋 PLAYER LEADERBOARD:");
                var sortedPlayers = context.Session.Players.Values
                    .OrderByDescending(p => p.Score)
                    .Take(5);
                
                int rank = 1;
                foreach (var player in sortedPlayers)
                {
                    string medal = rank switch
                    {
                        1 => "🥇",
                        2 => "🥈",
                        3 => "🥉",
                        _ => "  "
                    };
                    context.Log($"{medal} #{rank} {player.Name}: {player.Score} points ({player.FishesPulledIn} fish)");
                    rank++;
                }
            }
            
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        public string GetDescription()
        {
            return "Displays current game statistics and leaderboard";
        }
    }
}
