using Microsoft.AspNetCore.Mvc;
using Api.Models.Prototype;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FishPrototypeController : ControllerBase
    {
        [HttpGet("demo")]
        public IActionResult DemonstrateFishPrototypes()
        {
            var fishTypes = new List<(string Name, FishPrototype Prototype)>
            {
                ("BlueFish", new BlueFishShallow { PositionX = 100, PositionY = 200, Movement = new FishMovementBehavior("Random", 2.0, 0.5, -0.3) }),
                ("YellowFish", new YellowFishShallow { PositionX = 150, PositionY = 250, Movement = new FishMovementBehavior("Targeted", 3.0, 0.7, 0.2) }),
                ("BlackFish", new BlackFishShallow { PositionX = 200, PositionY = 300, Movement = new FishMovementBehavior("Circular", 1.5, 0.3, 0.4) }),
                ("BombFish", new BombFishShallow { PositionX = 250, PositionY = 350, Movement = new FishMovementBehavior("Random", 0.5, 0.1, 0.2) }),
                ("FatFish", new FatFishShallow { PositionX = 300, PositionY = 400, Movement = new FishMovementBehavior("Slow", 0.8, 0.2, -0.1) })
            };
            int fishNum = 1;
            foreach (var (name, prototype) in fishTypes)
            {
                var originalShallow = prototype;
                var shallowClone = originalShallow.CloneShallow();
                if (shallowClone.Movement != null)
                {
                    var originalSpeed = originalShallow.Movement?.Speed ?? 0;
                    shallowClone.Movement.Speed = 99.9;
                    var newOriginalSpeed = originalShallow.Movement?.Speed ?? 0;
                }
                var prototypeDeep = CreateDeepPrototype(name, prototype);
                var deepClone = prototypeDeep.CloneDeep();
                if (deepClone.Movement != null)
                {
                    var originalSpeed = prototypeDeep.Movement?.Speed ?? 0;
                    deepClone.Movement.Speed = 88.8;
                    var newOriginalSpeed = prototypeDeep.Movement?.Speed ?? 0;
                }
                fishNum++;
            }
            return Ok(new
            {
                success = true,
                message = "Fish Prototype Pattern Demonstration Complete",
            });
        }
        private FishPrototype CreateDeepPrototype(string fishType, FishPrototype original)
        {
            return fishType switch
            {
                "BlueFish" => new BlueFishDeep { PositionX = original.PositionX, PositionY = original.PositionY, Movement = original.Movement?.DeepClone() },
                "YellowFish" => new YellowFishDeep { PositionX = original.PositionX, PositionY = original.PositionY, Movement = original.Movement?.DeepClone() },
                "BlackFish" => new BlackFishDeep { PositionX = original.PositionX, PositionY = original.PositionY, Movement = original.Movement?.DeepClone() },
                "BombFish" => new BombFishDeep { PositionX = original.PositionX, PositionY = original.PositionY, Movement = original.Movement?.DeepClone() },
                "FatFish" => new FatFishDeep { PositionX = original.PositionX, PositionY = original.PositionY, Movement = original.Movement?.DeepClone() },
                _ => original
            };
        }
    }
}
