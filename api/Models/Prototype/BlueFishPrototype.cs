using Api.Models.Prototype;

namespace Api.Models.Prototype
{
    public class BlueFishShallow : FishPrototype
    {
        public BlueFishShallow()
        {
            this.Color = "#3A18B5";
            this.MovementSpeed = 1.0;
            this.Points = 5;
            this.Radius = 10.0;
            this.HasBeenHooked = false;
        }
        public override FishPrototype CloneShallow()
        {
            return new BlueFishShallow
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

            return new BlueFishShallow
            {
                Id = this.Id,
                PositionX = this.PositionX,
                PositionY = this.PositionY,
                Points = this.Points,
                Radius = this.Radius,
                Color = this.Color,
                MovementSpeed = this.MovementSpeed,
                HasBeenHooked = this.HasBeenHooked,
                Movement = clonedMovement!  
            };
        }
    }

    public class BlueFishDeep : FishPrototype
    {
        public BlueFishDeep()
        {
            this.Color = "#3A18B5";
            this.MovementSpeed = 1.0;
            this.Points = 5;
            this.Radius = 10.0;
            this.HasBeenHooked = false;
        }
        public override FishPrototype CloneShallow()
        {
            return new BlueFishDeep
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

            return new BlueFishDeep
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
