using System.Diagnostics;
using Api.Models;
using Api.Models.Bridge;
using System.Collections.Generic;
using System;

namespace Api.Models.Flyweight
{
    public class FishSharedData
    {
        public string Texture { get; }
        public string BaseColor { get; }
        public double BasePoints { get; }
        public double BaseRadius { get; }
        public double BaseSpeed { get; }
        public IFishMove MovementPattern { get; }
        public IFishBehavior Behavior { get; }

        public FishSharedData(string texture, string baseColor, double basePoints,
                     double baseRadius, double baseSpeed,
                     IFishMove movementPattern, IFishBehavior behavior)
                    {
                        Texture = texture;
                        BaseColor = baseColor;
                        BasePoints = basePoints;
                        BaseRadius = baseRadius;
                        BaseSpeed = baseSpeed;
                        MovementPattern = movementPattern;
                        Behavior = behavior;
                    }
    }

    public class FishFlyweightFactory
    {
        private static Dictionary<string, FishSharedData> _flyweights = new();

        public static FishSharedData GetFlyweight(string fishType)
        {
            if (!_flyweights.ContainsKey(fishType))
            {
                Console.WriteLine($"🔄 Creating Flyweight for: {fishType}");
                _flyweights[fishType] = CreateSharedData(fishType);
            }
            return _flyweights[fishType];
        }

        private static FishSharedData CreateSharedData(string fishType)
        {
            return fishType switch
            {
                "BlackFish" => new FishSharedData(
                    texture: LoadTexture("black_fish.png"),
                    baseColor: "#000000",
                    basePoints: 15,
                    baseRadius: 15.0,
                    baseSpeed: 2.0,
                    movementPattern: new RandomMove(),
                    behavior: new PassiveBehavior()
                ),
                "BlueFish" => new FishSharedData(
                    texture: LoadTexture("blue_fish.png"),
                    baseColor: "#3A18B5",
                    basePoints: 5,
                    baseRadius: 10.0,
                    baseSpeed: 1.0,
                    movementPattern: new RandomMove(),
                    behavior: new AggressiveBehavior()
                ),
                "BombFish" => new FishSharedData(
                    texture: LoadTexture("bomb_fish.png"),
                    baseColor: "#880000ff",
                    basePoints: 15,
                    baseRadius: 10.0,
                    baseSpeed: 0.0,
                    movementPattern: new RandomMove(),
                    behavior: new NocturnalBehavior()
                ),
                "YellowFish" => new FishSharedData(
                    texture: LoadTexture("yellow_fish.png"),
                    baseColor: "#D9E65A",
                    basePoints: 30,
                    baseRadius: 5.0,
                    baseSpeed: 5.0,
                    movementPattern: new RandomMove(),
                    behavior: new TerritorialBehavior()
                ),
                _ => throw new ArgumentException($"Unknown fish type: {fishType}")
            };
        }

        private static string LoadTexture(string filename)
        {
            return $"{filename}_texture_data";
        }

        public static void PrintMemoryStatistics()
        {
            Console.WriteLine("📊 FLYWEIGHT MEMORY STATISTICS:");
            int totalFish = 1000;
            double memoryPerTexture = 5.0;
            double memoryPerFish = 0.001;
            double withoutFlyweight = totalFish * memoryPerTexture;
            double withFlyweight = (_flyweights.Count * memoryPerTexture) + (totalFish * memoryPerFish);
            
            Console.WriteLine($"Without: {withoutFlyweight:F2} MB, With: {withFlyweight:F2} MB");
            Console.WriteLine($"Saved: {(withoutFlyweight - withFlyweight):F2} MB ({(withoutFlyweight - withFlyweight) / withoutFlyweight * 100:F1}%)");
        }
        public static void TestPerformance()
        {
            Console.WriteLine("🧪 FLYWEIGHT PERFORMANCE TEST");
            Console.WriteLine("==============================");
            
            int fishCount = 1000;
            var random = new Random();
            string[] fishTypes = { "BlackFish", "BlueFish", "BombFish", "YellowFish" };
            
            // Test WITHOUT Flyweight (simulated)
            Console.WriteLine("\n❌ WITHOUT FLYWEIGHT:");
            var stopwatch = Stopwatch.StartNew();
            long memoryBefore = GC.GetTotalMemory(true);
            
            // Simulate creating 1000 fish without flyweight
            var regularFishList = new List<object>();
            for (int i = 0; i < fishCount; i++)
            {
                string type = fishTypes[random.Next(fishTypes.Length)];
                // Each fish creates its own shared data
                var fakeSharedData = CreateSharedData(type); // NEW instance each time
                regularFishList.Add(new { Type = type, Data = fakeSharedData });
            }
            
            stopwatch.Stop();
            long memoryAfter = GC.GetTotalMemory(false);
            long memoryUsedRegular = memoryAfter - memoryBefore;
            
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Memory: {memoryUsedRegular / 1024} KB");
            Console.WriteLine($"Objects created: {fishCount * 2} (fish + shared data)");
            
            // Clear for GC
            regularFishList.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            // Test WITH Flyweight
            Console.WriteLine("\n✅ WITH FLYWEIGHT:");
            stopwatch.Restart();
            memoryBefore = GC.GetTotalMemory(true);
            
            var flyweightFishList = new List<object>();
            for (int i = 0; i < fishCount; i++)
            {
                string type = fishTypes[random.Next(fishTypes.Length)];
                // Reuse shared data from flyweight
                var sharedData = GetFlyweight(type); // REUSES existing
                flyweightFishList.Add(new { Type = type, Data = sharedData });
            }
            
            stopwatch.Stop();
            memoryAfter = GC.GetTotalMemory(false);
            long memoryUsedFlyweight = memoryAfter - memoryBefore;
            
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Memory: {memoryUsedFlyweight / 1024} KB");
            Console.WriteLine($"Objects created: {fishCount + _flyweights.Count} (fish + {_flyweights.Count} shared flyweights)");
            
            // Show comparison
            Console.WriteLine("\n📊 COMPARISON RESULTS:");
            Console.WriteLine($"Time saved: {stopwatch.ElapsedMilliseconds} ms faster");
            Console.WriteLine($"Memory saved: {(memoryUsedRegular - memoryUsedFlyweight) / 1024} KB ({(1 - (double)memoryUsedFlyweight / memoryUsedRegular) * 100:F1}% less)");
            Console.WriteLine($"Objects saved: {fishCount - _flyweights.Count} fewer objects");
            
            // Print factory stats
            Console.WriteLine($"\n🏭 FLYWEIGHT FACTORY STATS:");
            Console.WriteLine($"Total flyweights created: {_flyweights.Count}");
            foreach (var kvp in _flyweights)
            {
                Console.WriteLine($"  - {kvp.Key}");
            }
        }
    }
}