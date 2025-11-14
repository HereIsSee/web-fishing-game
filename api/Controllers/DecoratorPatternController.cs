using Microsoft.AspNetCore.Mvc;
using Api.Models.Decorator;
using Api.Models;
using Api.Models.Bridge;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecoratorPatternController : ControllerBase
    {
        [HttpGet("demo")]
        public IActionResult DemonstrateDecoratorPattern()
        {
            var normalDecorator = new NormalFishDecorator();
            var weightedDecorator = new WeightedFishDecorator();
            var poisonedDecorator = new PoisonedFishDecorator();
            return Ok();
        }
        [HttpGet("all-combinations")]
        public IActionResult ShowAllCombinations()
        {
            var combinations = new System.Collections.Generic.List<object>();
            var fishTypes = new[] { "BlueFish", "BlackFish", "YellowFish", "BombFish", "FatFish" };
            var decorators = new (string name, IFishDecorator decorator)[]
            {
                ("Normal", new NormalFishDecorator()),
                ("Weighted", new WeightedFishDecorator()),
                ("Poisoned", new PoisonedFishDecorator())
            };
            int basePoints = 100; 
            foreach (var fishType in fishTypes)
            {
                foreach (var (decoratorName, decorator) in decorators)
                {
                    int effectivePoints = (int)(basePoints * decorator.GetPointsMultiplier());
                    string effect = decorator switch
                    {
                        NormalFishDecorator => "No effect",
                        WeightedFishDecorator => $"Slow 50% for {decorator.GetSlowdownDurationSeconds()}s, +50% points",
                        PoisonedFishDecorator => $"Freeze hook for {decorator.GetFreezeDurationSeconds()}s, -{(int)(basePoints * decorator.GetPointsPenalty())} points",
                        _ => "Unknown"
                    };
                    combinations.Add(new
                    {
                        FishType = fishType,
                        Decorator = decoratorName,
                        BasePoints = basePoints,
                        EffectivePoints = effectivePoints,
                        Effect = effect
                    });
                }
            }

            return Ok(new
            {
                TotalCombinations = combinations.Count,
                ClassesNeededWithoutDecorator = "15 classes (5 types × 3 decorators)",
                ClassesNeededWithDecorator = "8 classes (5 types + 3 decorators)",
                Combinations = combinations
            });
        }
    }
}
