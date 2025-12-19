namespace Api.Models.Visitor
{
    using Api.Models.Fishes;

    /// <summary>
    /// VISITOR PATTERN - Concrete Visitor 3
    /// Checks if catching this fish unlocks achievements and tracks progress
    /// </summary>
    public class AchievementVisitor : IFishVisitor
    {
        public List<string> UnlockedAchievements { get; private set; } = new();
        public Dictionary<string, int> ProgressUpdates { get; private set; } = new();
        public string AchievementCategory { get; private set; } = string.Empty;
        public int ExperiencePoints { get; private set; }

        // Static counters to track overall progress across all catches
        private static Dictionary<string, int> _globalFishCounts = new()
        {
            ["BlackFish"] = 0,
            ["BlueFish"] = 0,
            ["YellowFish"] = 0,
            ["BombFish"] = 0,
            ["FatFish"] = 0
        };

        public void Visit(BlackFish fish)
        {
            _globalFishCounts["BlackFish"]++;
            int count = _globalFishCounts["BlackFish"];
            
            AchievementCategory = "Rare Hunter";
            ExperiencePoints = 50;
            
            ProgressUpdates["Speed Demon"] = count;
            
            if (count == 1)
            {
                UnlockedAchievements.Add("🖤 First Rare Catch - Caught your first BlackFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: First Rare Catch!");
            }
            
            if (count == 5)
            {
                UnlockedAchievements.Add("⚡ Speed Hunter - Caught 5 BlackFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Speed Hunter (5 BlackFish)!");
            }
            
            if (count == 10)
            {
                UnlockedAchievements.Add("🌟 Speed Demon - Mastered catching fast fish (10 BlackFish)!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Speed Demon (10 BlackFish)!");
            }
            
            Console.WriteLine($"🎯 AchievementVisitor: BlackFish #{count} → {ExperiencePoints} XP (Category: {AchievementCategory})");
        }

        public void Visit(BlueFish fish)
        {
            _globalFishCounts["BlueFish"]++;
            int count = _globalFishCounts["BlueFish"];
            
            AchievementCategory = "Beginner";
            ExperiencePoints = 10;
            
            ProgressUpdates["Common Collector"] = count;
            
            if (count == 1)
            {
                UnlockedAchievements.Add("🎣 Beginner's Luck - Caught your first fish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Beginner's Luck!");
            }
            
            if (count == 20)
            {
                UnlockedAchievements.Add("💙 Blue Collector - Caught 20 BlueFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Blue Collector (20 BlueFish)!");
            }
            
            if (count == 50)
            {
                UnlockedAchievements.Add("🌊 Ocean Master - Caught 50 BlueFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Ocean Master (50 BlueFish)!");
            }
            
            Console.WriteLine($"🎯 AchievementVisitor: BlueFish #{count} → {ExperiencePoints} XP (Category: {AchievementCategory})");
        }

        public void Visit(YellowFish fish)
        {
            _globalFishCounts["YellowFish"]++;
            int count = _globalFishCounts["YellowFish"];
            
            AchievementCategory = "Treasure Hunter";
            ExperiencePoints = 30;
            
            ProgressUpdates["Golden Touch"] = count;
            
            if (count == 1)
            {
                UnlockedAchievements.Add("💛 Golden Opportunity - Caught your first YellowFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Golden Opportunity!");
            }
            
            if (count == 5)
            {
                UnlockedAchievements.Add("✨ Golden Touch - Caught 5 YellowFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Golden Touch (5 YellowFish)!");
            }
            
            if (count == 15)
            {
                UnlockedAchievements.Add("👑 Gold Rush - Master of yellow fish (15 YellowFish)!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Gold Rush (15 YellowFish)!");
            }
            
            Console.WriteLine($"🎯 AchievementVisitor: YellowFish #{count} → {ExperiencePoints} XP (Category: {AchievementCategory})");
        }

        public void Visit(BombFish fish)
        {
            _globalFishCounts["BombFish"]++;
            int count = _globalFishCounts["BombFish"];
            
            AchievementCategory = "Daredevil";
            ExperiencePoints = 100; // High XP for risk
            
            ProgressUpdates["Risk Taker"] = count;
            
            if (count == 1)
            {
                UnlockedAchievements.Add("💣 Risk Taker - You're brave (or crazy) enough to catch a BombFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Risk Taker!");
            }
            
            if (count == 3)
            {
                UnlockedAchievements.Add("🎰 Adrenaline Junkie - Caught 3 BombFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Adrenaline Junkie (3 BombFish)!");
            }
            
            if (count == 10)
            {
                UnlockedAchievements.Add("💥 Demolition Expert - Mastered the danger (10 BombFish)!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Demolition Expert (10 BombFish)!");
            }
            
            Console.WriteLine($"🎯 AchievementVisitor: BombFish #{count} → {ExperiencePoints} XP (Category: {AchievementCategory}) ⚠️");
        }

        public void Visit(FatFish fish)
        {
            _globalFishCounts["FatFish"]++;
            int count = _globalFishCounts["FatFish"];
            
            AchievementCategory = "Legendary";
            ExperiencePoints = 75;
            
            ProgressUpdates["Whale Hunter"] = count;
            
            if (count == 1)
            {
                UnlockedAchievements.Add("🐋 Big Catch - Caught your first FatFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Big Catch!");
            }
            
            if (count == 3)
            {
                UnlockedAchievements.Add("🎣 Whale Hunter - Caught 3 rare FatFish!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Whale Hunter (3 FatFish)!");
            }
            
            if (count == 10)
            {
                UnlockedAchievements.Add("👑 Leviathan Master - Legendary FatFish master (10 catches)!");
                Console.WriteLine($"🏆 ACHIEVEMENT UNLOCKED: Leviathan Master (10 FatFish)!");
            }
            
            Console.WriteLine($"🎯 AchievementVisitor: FatFish #{count} → {ExperiencePoints} XP (Category: {AchievementCategory})");
        }

        // Method to get total catches across all fish types
        public static int GetTotalCatches()
        {
            return _globalFishCounts.Values.Sum();
        }

        // Method to reset all progress (useful for testing)
        public static void ResetProgress()
        {
            foreach (var key in _globalFishCounts.Keys.ToList())
            {
                _globalFishCounts[key] = 0;
            }
            Console.WriteLine("🔄 Achievement progress reset");
        }

        // Method to get detailed statistics
        public static Dictionary<string, int> GetStatistics()
        {
            return new Dictionary<string, int>(_globalFishCounts);
        }
    }
}
