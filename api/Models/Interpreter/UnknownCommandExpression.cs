namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Terminal Expression
    /// Handles unknown/invalid commands
    /// </summary>
    public class UnknownCommandExpression : IExpression
    {
        private readonly string _commandText;

        public UnknownCommandExpression(string commandText)
        {
            _commandText = commandText;
        }

        public void Interpret(GameAdminContext context)
        {
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            context.Log($"❌ Unknown command: '{_commandText}'");
            context.Log("   Type 'help' to see available commands");
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        public string GetDescription()
        {
            return $"Unknown command: {_commandText}";
        }
    }
}
