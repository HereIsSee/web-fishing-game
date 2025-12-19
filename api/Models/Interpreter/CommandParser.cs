namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Parser
    /// Parses text commands and creates corresponding expression objects
    /// </summary>
    public class CommandParser
    {
        public IExpression? Parse(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                return null;

            var parts = commandText.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
                return null;

            try
            {
                return parts[0] switch
                {
                    "score" when parts.Length >= 2 && parts[1] == "reset" 
                        => new ScoreResetExpression(),
                    
                    "spawn" when parts.Length >= 4 && parts[1] == "fish" 
                        => new SpawnFishExpression(parts[2], int.Parse(parts[3])),
                    
                    "session" when parts.Length >= 2 && parts[1] == "start" 
                        => new SessionStartExpression(),
                    
                    "session" when parts.Length >= 2 && parts[1] == "stop" 
                        => new SessionStopExpression(),
                    
                    "show" when parts.Length >= 2 && parts[1] == "stats" 
                        => new ShowStatsExpression(),
                    
                    "help" 
                        => new HelpExpression(),
                    
                    _ => new UnknownCommandExpression(commandText)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Parse error: {ex.Message}");
                return new UnknownCommandExpression(commandText);
            }
        }

        public List<string> GetAvailableCommands()
        {
            return new List<string>
            {
                "score reset                    - Reset all player scores",
                "spawn fish [type] [count]      - Spawn fish (types: blue, black, yellow, bomb, fat)",
                "session start                  - Start the game session",
                "session stop                   - Stop the game session",
                "show stats                     - Display game statistics",
                "help                           - Show this help message",
                "exit                           - Exit command interpreter"
            };
        }
    }
}
