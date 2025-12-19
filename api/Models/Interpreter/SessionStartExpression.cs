namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Terminal Expression
    /// Command: "session start" - Starts the game session
    /// </summary>
    public class SessionStartExpression : IExpression
    {
        public void Interpret(GameAdminContext context)
        {
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            context.Log("Executing: SESSION START");

            if (context.Session.IsActive)
            {
                context.Log("⚠️ Session is already active!");
            }
            else
            {
                context.Session.IsActive = true;
                context.Session.State = GameState.Playing;
                context.Session.StartTime = DateTime.UtcNow;
                
                context.Log("✅ Game session started");
                context.Log($"   Players: {context.Session.Players.Count}");
                context.Log($"   Fish: {context.Session.Environment.GetAllFishesFlat().Count}");
                context.Log($"   Start time: {context.Session.StartTime:HH:mm:ss}");
            }
            
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        public string GetDescription()
        {
            return "Starts the game session";
        }
    }
}
