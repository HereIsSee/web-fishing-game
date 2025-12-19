using Api.Models.Prototype;

namespace Api.Models.Prototype
{
    public class YellowFishShallow : FishPrototype
    {
        public YellowFishShallow()
        {
            this.Color = "#FFD700";
            this.MovementSpeed = 2.0;
            this.Points = 15;
            this.Radius = 12.0;
            this.HasBeenHooked = false;
        }

        public override FishPrototype CloneShallow()
        {
            return new YellowFishShallow
            {
                Id = this.Id,
                PositionX = this.PositionX,
                PositionY = this.PositionY,
                Points = this.Points,
                Radius = this.Radius,
                Color = this.Color,
                MovementSpeed = this.MovementSpeed,
                HasBeenHooked = this.HasBeenHooked,
                Movement = this.Movement!  
            };
        }

        public override FishPrototype CloneDeep()
        {
            var clonedMovement = this.Movement != null
                ? this.Movement.DeepClone()
                : null;

            return new YellowFishShallow
            {
                Id = this.Id,
                PositionX = this.PositionX,
                PositionY = this.PositionY,
                Points = this.Points,
                Radius = this.Radius,
                Color = this.Color,
                MovementSpeed = this.MovementSpeed,
                HasBeenHooked = this.HasBeenHooked,
                Movement = clonedMovement 
            };
        }
    }
    public class YellowFishDeep : FishPrototype
    {
        public YellowFishDeep()
        {
            this.Color = "#FFD700";
            this.MovementSpeed = 2.0;
            this.Points = 15;
            this.Radius = 12.0;
            this.HasBeenHooked = false;
        }

        public override FishPrototype CloneShallow()
        {
            return new YellowFishDeep
            {
                Id = this.Id,
                PositionX = this.PositionX,
                PositionY = this.PositionY,
                Points = this.Points,
                Radius = this.Radius,
                Color = this.Color,
                MovementSpeed = this.MovementSpeed,
                HasBeenHooked = this.HasBeenHooked,
                Movement = this.Movement
            };
        }

        public override FishPrototype CloneDeep()
        {
            var clonedMovement = this.Movement != null
                ? this.Movement.DeepClone()
                : null;

            return new YellowFishDeep
            {
                Id = this.Id,
                PositionX = this.PositionX,
                PositionY = this.PositionY,
                Points = this.Points,
                Radius = this.Radius,
                Color = this.Color,
                MovementSpeed = this.MovementSpeed,
                HasBeenHooked = this.HasBeenHooked,
                Movement = clonedMovement
            };
        }
    }
}
