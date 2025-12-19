namespace Api.Models.ChainOfResponsibility
{
    /// <summary>
    /// Context for catch attempt validation chain
    /// </summary>
    public class CatchAttemptContext
    {
        public Player Player { get; set; }
        public Fish Fish { get; set; }
        public Session Session { get; set; }
        public bool IsValid { get; set; } = true;
        public string FailureReason { get; set; } = string.Empty;
        public List<string> ValidationLog { get; set; } = new List<string>();

        public CatchAttemptContext(Player player, Fish fish, Session session)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Fish = fish ?? throw new ArgumentNullException(nameof(fish));
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public void AddLog(string message)
        {
            ValidationLog.Add(message);
            Console.WriteLine($"🔗 Chain: {message}");
        }

        public void Fail(string reason)
        {
            IsValid = false;
            FailureReason = reason;
            AddLog($"❌ FAILED: {reason}");
        }
    }
}
