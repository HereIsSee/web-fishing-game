namespace Api.Models
{
    public class Boat
    {
        public int Id { get; set; }

        //Type should be an enum
        public string Type { get; set; } = string.Empty;

        public double MovementSpeed { get; set; }
    
        public Player? Player { get; set; }
    }
}
