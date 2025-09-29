namespace Api.Models
{
    public class Scoreboard
    {
        public int Id { get; set; }

        // Scoreboard belongs to a game and vice versa
        public Game? Game { get; set; }
        
    }
}
