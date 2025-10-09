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
        public Boat Boat { get; set; }

        // Player has a fishing rod
        public FishingRod FishingRod { get; set; } = null!;

    }
}
