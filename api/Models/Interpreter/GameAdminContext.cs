namespace Api.Models.Interpreter
{
    /// <summary>
    /// Context for command interpretation
    /// Contains game state references needed by commands
    /// </summary>
    public class GameAdminContext
    {
        public Session Session { get; set; }
        public Dictionary<string, object> Variables { get; set; } = new();
        public List<string> OutputLog { get; set; } = new();

        public GameAdminContext(Session session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public void Log(string message)
        {
            OutputLog.Add(message);
            Console.WriteLine($"🎮 {message}");
        }

        public void SetVariable(string name, object value)
        {
            Variables[name] = value;
            Log($"Variable set: {name} = {value}");
        }

        public object? GetVariable(string name)
        {
            return Variables.TryGetValue(name, out var value) ? value : null;
        }
    }
}
