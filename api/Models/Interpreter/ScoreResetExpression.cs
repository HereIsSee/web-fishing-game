namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Terminal Expression
    /// Command: "score reset" - Resets all player scores
    /// </summary>
    public class ScoreResetExpression : IExpression
    {
        public void Interpret(GameAdminContext context)
        {
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            context.Log("Executing: SCORE RESET");
            
            context.Session.Scoreboard.PlayerScores.Clear();
            
            foreach (var player in context.Session.Players.Values)
            {
                player.Score = 0;
                player.FishesPulledIn = 0;
            }
            
            context.Log($"✅ All player scores reset to 0");
            context.Log($"   Total players affected: {context.Session.Players.Count}");
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        public string GetDescription()
        {
            return "Resets all player scores to 0";
        }
    }
}
