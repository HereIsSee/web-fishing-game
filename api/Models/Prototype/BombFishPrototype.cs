using Api.Models.Prototype;

namespace Api.Models.Prototype
{
    public class BombFishShallow : FishPrototype
    {
        public BombFishShallow()
        {
            this.Color = "#880000FF";
            this.MovementSpeed = 0.0;
            this.Points = 15;
            this.Radius = 10.0;
            this.HasBeenHooked = false;
        }

        public override FishPrototype CloneShallow()
        {
            return new BombFishShallow
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

            return new BombFishShallow
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

    public class BombFishDeep : FishPrototype
    {
        public BombFishDeep()
        {
            this.Color = "#880000FF";
            this.MovementSpeed = 0.0;
            this.Points = 15;
            this.Radius = 10.0;
            this.HasBeenHooked = false;
        }

        public override FishPrototype CloneShallow()
        {
            return new BombFishDeep
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

            return new BombFishDeep
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
