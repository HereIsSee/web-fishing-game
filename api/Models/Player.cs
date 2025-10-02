namespace Api.Models
{
    public class Player
    {
        public int Id { get; set; }

        public string ConnectionId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Score { get; set; } = 0;

        public int FishesPulledIn { get; set; } = 0;

        // Player has a boat
        public int BoatId { get; set; }
        public Boat? Boat { get; set; }

        // Player has a fishing rod
        public int FishingRodId { get; set; }
        public FishingRod? FishingRod { get; set; }

        // Player belongs to a game
        public Game? Game { get; set; }
        public int GameId { get; set; }
        
        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}
