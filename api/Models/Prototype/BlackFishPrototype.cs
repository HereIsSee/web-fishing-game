using Api.Models.Prototype;

namespace Api.Models.Prototype
{
    public class BlackFishShallow : FishPrototype
    {
        public BlackFishShallow()
        {
            this.Color = "#000000";
            this.MovementSpeed = 1.5;
            this.Points = 10;
            this.Radius = 9.0;
            this.HasBeenHooked = false;
        }
        public override FishPrototype CloneShallow()
        {
            return new BlackFishShallow
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

            return new BlackFishShallow
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
    public class BlackFishDeep : FishPrototype
    {
        public BlackFishDeep()
        {
            this.Color = "#000000";
            this.MovementSpeed = 1.5;
            this.Points = 10;
            this.Radius = 9.0;
            this.HasBeenHooked = false;
        }
        public override FishPrototype CloneShallow()
        {
            return new BlackFishDeep
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

            return new BlackFishDeep
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
