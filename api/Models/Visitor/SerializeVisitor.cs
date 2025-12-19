namespace Api.Models.Visitor
{
    using Api.Models.Fishes;

    /// <summary>
    /// VISITOR PATTERN - Concrete Visitor 3
    /// Serializes fish data into DTO for network transmission (SignalR)
    /// </summary>
    public class SerializeVisitor : IFishVisitor
    {
        public Dictionary<string, object> SerializedData { get; private set; } = new();

        public void Visit(BlackFish fish)
        {
            SerializedData = new Dictionary<string, object>
            {
                ["id"] = fish.Id,
                ["type"] = "black",
                ["x"] = fish.PositionX,
                ["y"] = fish.PositionY,
                ["radius"] = fish.Radius,
                ["color"] = fish.Color,
                ["points"] = fish.Points,
                ["speed"] = fish.MovementSpeed,
                ["hooked"] = fish.HasBeenHooked,
                ["rarity"] = "rare",
                ["specialAbility"] = "fast_movement"
            };
            
            Console.WriteLine($"📦 SerializeVisitor: BlackFish serialized (ID: {fish.Id}, Type: rare)");
        }

        public void Visit(BlueFish fish)
        {
            SerializedData = new Dictionary<string, object>
            {
                ["id"] = fish.Id,
                ["type"] = "blue",
                ["x"] = fish.PositionX,
                ["y"] = fish.PositionY,
                ["radius"] = fish.Radius,
                ["color"] = fish.Color,
                ["points"] = fish.Points,
                ["speed"] = fish.MovementSpeed,
                ["hooked"] = fish.HasBeenHooked,
                ["rarity"] = "common",
                ["specialAbility"] = "none"
            };
            
            Console.WriteLine($"📦 SerializeVisitor: BlueFish serialized (ID: {fish.Id}, Type: common)");
        }

        public void Visit(YellowFish fish)
        {
            SerializedData = new Dictionary<string, object>
            {
                ["id"] = fish.Id,
                ["type"] = "yellow",
                ["x"] = fish.PositionX,
                ["y"] = fish.PositionY,
                ["radius"] = fish.Radius,
                ["color"] = fish.Color,
                ["points"] = fish.Points,
                ["speed"] = fish.MovementSpeed,
                ["hooked"] = fish.HasBeenHooked,
                ["rarity"] = "uncommon",
                ["specialAbility"] = "bonus_points"
            };
            
            Console.WriteLine($"📦 SerializeVisitor: YellowFish serialized (ID: {fish.Id}, Type: uncommon)");
        }

        public void Visit(BombFish fish)
        {
            SerializedData = new Dictionary<string, object>
            {
                ["id"] = fish.Id,
                ["type"] = "bomb",
                ["x"] = fish.PositionX,
                ["y"] = fish.PositionY,
                ["radius"] = fish.Radius,
                ["color"] = fish.Color,
                ["points"] = fish.Points,
                ["speed"] = fish.MovementSpeed,
                ["hooked"] = fish.HasBeenHooked,
                ["rarity"] = "danger",
                ["specialAbility"] = "explode_on_catch",
                ["warning"] = "⚠️ EXPLOSIVE"
            };
            
            Console.WriteLine($"📦 SerializeVisitor: BombFish serialized (ID: {fish.Id}, Type: DANGER) ⚠️");
        }

        public void Visit(FatFish fish)
        {
            SerializedData = new Dictionary<string, object>
            {
                ["id"] = fish.Id,
                ["type"] = "fat",
                ["x"] = fish.PositionX,
                ["y"] = fish.PositionY,
                ["radius"] = fish.Radius,
                ["color"] = fish.Color,
                ["points"] = fish.Points,
                ["speed"] = fish.MovementSpeed,
                ["hooked"] = fish.HasBeenHooked,
                ["rarity"] = "epic",
                ["specialAbility"] = "high_value",
                ["weight"] = "heavy"
            };
            
            Console.WriteLine($"📦 SerializeVisitor: FatFish serialized (ID: {fish.Id}, Type: epic)");
        }

        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(SerializedData, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }
    }
}
