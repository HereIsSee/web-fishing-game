using Api.Models.Fishes;

namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Terminal Expression with parameters
    /// Command: "spawn fish [type] [count]" - Spawns fish of specified type
    /// Example: "spawn fish blue 5"
    /// </summary>
    public class SpawnFishExpression : IExpression
    {
        private readonly string _fishType;
        private readonly int _count;

        public SpawnFishExpression(string fishType, int count)
        {
            _fishType = fishType?.ToLower() ?? throw new ArgumentNullException(nameof(fishType));
            _count = count;
        }

        public void Interpret(GameAdminContext context)
        {
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            context.Log($"Executing: SPAWN FISH ({_fishType} x{_count})");

            var random = new Random();
            int spawned = 0;

            for (int i = 0; i < _count; i++)
            {
                double x = random.Next(100, context.Session.Environment.Width - 100);
                double y = random.Next(100, context.Session.Environment.WaterLevelHeight - 100);

                Fish? newFish = _fishType switch
                {
                    "blue" => new BlueFish(x, y),
                    "black" => new BlackFish(x, y),
                    "yellow" => new YellowFish(x, y),
                    "bomb" => new BombFish(x, y),
                    "fat" => new FatFish(x, y),
                    _ => null
                };

                if (newFish != null)
                {
                    var fishLeaf = new FishLeaf(newFish);
                    context.Session.Environment.FishGroups.Add(fishLeaf);
                    spawned++;
                }
            }

            context.Log($"✅ Spawned {spawned} {_fishType} fish");
            context.Log($"   Total fish in environment: {context.Session.Environment.GetAllFishesFlat().Count}");
            context.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }

        public string GetDescription()
        {
            return $"Spawns {_count} {_fishType} fish in the game environment";
        }
    }
}
