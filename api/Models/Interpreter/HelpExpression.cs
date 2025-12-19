namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Terminal Expression
    /// Command: "help" - Shows available commands
    /// </summary>
    public class HelpExpression : IExpression
    {
        public void Interpret(GameAdminContext context)
        {
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            context.Log("📖 AVAILABLE COMMANDS (Interpreter Pattern)");
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            context.Log("");
            
            var parser = new CommandParser();
            foreach (var cmd in parser.GetAvailableCommands())
            {
                context.Log($"  {cmd}");
            }
            
            context.Log("");
            context.Log("Example usage:");
            context.Log("  > spawn fish blue 10");
            context.Log("  > session start");
            context.Log("  > show stats");
            context.Log("  > score reset");
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        public string GetDescription()
        {
            return "Shows list of available commands";
        }
    }
}
