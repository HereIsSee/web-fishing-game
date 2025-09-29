namespace Api.Models
{
    public class Session
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public DateTime? EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        // Unique code for joining the session.
        // probably not going to use it for our project
        // just join all player to one session that is
        // created as soon as the project is launched
        public string SessionCode { get; set; } = string.Empty;

        // Session has a Game 1 to 1
        public Game? Game { get; set; }
        public int GameId { get; set; }
    }

}
