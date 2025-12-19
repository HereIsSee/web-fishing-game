namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Terminal Expression
    /// Command: "session stop" - Stops the game session
    /// </summary>
    public class SessionStopExpression : IExpression
    {
        public void Interpret(GameAdminContext context)
        {
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            context.Log("Executing: SESSION STOP");

            if (!context.Session.IsActive)
            {
                context.Log("⚠️ Session is not active!");
            }
            else
            {
                context.Session.IsActive = false;
                context.Session.State = GameState.Finished;
                context.Session.EndTime = DateTime.UtcNow;
                
                var duration = context.Session.EndTime.Value - context.Session.StartTime;
                
                context.Log("✅ Game session stopped");
                context.Log($"   Duration: {duration.TotalMinutes:F1} minutes");
                context.Log($"   End time: {context.Session.EndTime:HH:mm:ss}");
            }
            
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        public string GetDescription()
        {
            return "Stops the game session";
        }
    }
}
