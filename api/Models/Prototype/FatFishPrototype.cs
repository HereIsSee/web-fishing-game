using Api.Models.Prototype;

namespace Api.Models.Prototype
{
    public class FatFishShallow : FishPrototype
    {
        public FatFishShallow()
        {
            this.Color = "#FF6B6B";
            this.MovementSpeed = 0.5;
            this.Points = 25;
            this.Radius = 15.0;
            this.HasBeenHooked = false;
        }
        public override FishPrototype CloneShallow()
        {
            return new FatFishShallow
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

            return new FatFishShallow
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
    public class FatFishDeep : FishPrototype
    {
        public FatFishDeep()
        {
            this.Color = "#FF6B6B";
            this.MovementSpeed = 0.5;
            this.Points = 25;
            this.Radius = 15.0;
            this.HasBeenHooked = false;
        }

        public override FishPrototype CloneShallow()
        {
            return new FatFishDeep
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

            return new FatFishDeep
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
