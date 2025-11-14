using Microsoft.AspNetCore.Mvc;
using Api.Models.Bridge;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BridgePatternController : ControllerBase
    {
        [HttpGet("demo")]
        public IActionResult DemonstrateBridgePattern()
        {
            var blueFishAggressive = new BehavioralFish("BlueFish", 100, 150, new AggressiveBehavior());
            var blueFishPassive = new BehavioralFish("BlueFish", 110, 160, new PassiveBehavior());
            var fish = new BehavioralFish("BlackFish", 200, 250, new PassiveBehavior());
            fish.SetBehavior(new AggressiveBehavior());
            var fishTypes = new[] { "BlueFish", "BlackFish", "YellowFish", "BombFish", "FatFish" };
            var behaviors = new IFishBehavior[]
            {
                new AggressiveBehavior(),
                new PassiveBehavior(),
                new TerritorialBehavior(),
                new NocturnalBehavior()
            };
            var combinations = new System.Collections.Generic.List<object>();
            int count = 1;
            foreach (var fishType in fishTypes)
            {
                foreach (var behavior in behaviors)
                {
                    var bf = new BehavioralFish(fishType, 0, 0, behavior);
                    combinations.Add(new
                    {
                        Index = count++,
                        FishType = fishType,
                        Behavior = behavior.Behavior,
                        HuntingProbability = $"{behavior.GetHuntingProbability() * 100}%",
                        EscapeProbability = $"{behavior.GetEscapeProbability() * 100}%"
                    });
                }
            }
            return Ok(new
            {
                Message = "Bridge Pattern: Fish type independent from Fish behavior",
                Example1 = "BlueFish with AggressiveBehavior (80% hunt, 20% escape)",
                Example2 = "BlueFish with PassiveBehavior (30% hunt, 70% escape)",
                RuntimeChange = "BlackFish changed from PassiveBehavior to AggressiveBehavior",
                TotalCombinations = combinations.Count,
                ClassesWithoutBridge = "20 classes (5 types × 4 behaviors)",
                ClassesWithBridge = "9 classes (5 types + 4 behaviors)",
                AllCombinations = combinations
            });
        }
        [HttpGet("runtime-change")]
        public IActionResult TestRuntimeBehaviorChange()
        {
            var fish = new BehavioralFish("YellowFish", 150, 200, new AggressiveBehavior());
            var initialHunt = fish.GetBehavior().GetHuntingProbability() * 100;
            var initialEscape = fish.GetBehavior().GetEscapeProbability() * 100;
            fish.SetBehavior(new PassiveBehavior());
            var passiveHunt = fish.GetBehavior().GetHuntingProbability() * 100;
            var passiveEscape = fish.GetBehavior().GetEscapeProbability() * 100;
            fish.SetBehavior(new TerritorialBehavior());
            var territorialHunt = fish.GetBehavior().GetHuntingProbability() * 100;
            var territorialEscape = fish.GetBehavior().GetEscapeProbability() * 100;
            return Ok(new
            {
                Initial = new { Fish = "YellowFish", Behavior = "Aggressive", HuntingProbability = $"{initialHunt}%", EscapeProbability = $"{initialEscape}%" },
                AfterChange1 = new { Fish = "YellowFish", Behavior = "Passive", HuntingProbability = $"{passiveHunt}%", EscapeProbability = $"{passiveEscape}%" },
                AfterChange2 = new { Fish = "YellowFish", Behavior = "Territorial", HuntingProbability = $"{territorialHunt}%", EscapeProbability = $"{territorialEscape}%" }
            });
        }

        [HttpGet("combinations")]
        public IActionResult ShowAllCombinations()
        {
            var combinations = new System.Collections.Generic.List<object>();
            var fishTypes = new[] { "BlueFish", "BlackFish", "YellowFish", "BombFish", "FatFish" };
            var behaviors = new (string name, IFishBehavior behavior)[]
            {
                ("Aggressive", new AggressiveBehavior()),
                ("Passive", new PassiveBehavior()),
                ("Territorial", new TerritorialBehavior()),
                ("Nocturnal", new NocturnalBehavior())
            };
            foreach (var fishType in fishTypes)
            {
                foreach (var (behaviorName, behavior) in behaviors)
                {
                    var fish = new BehavioralFish(fishType, 0, 0, behavior);
                    combinations.Add(new
                    {
                        FishType = fishType,
                        Behavior = behaviorName,
                        HuntingProbability = $"{behavior.GetHuntingProbability() * 100}%",
                        EscapeProbability = $"{behavior.GetEscapeProbability() * 100}%",
                        Description = fish.GetCombinationDescription()
                    });
                }
            }
            return Ok(new
            {
                TotalCombinations = combinations.Count,
                ClassesNeededWithoutBridge = "20 classes (5 types × 4 behaviors)",
                ClassesNeededWithBridge = "9 classes (5 types + 4 behaviors)",
                Combinations = combinations
            });
        }
    }
}
