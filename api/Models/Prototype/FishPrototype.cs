using System.Runtime.InteropServices;

namespace Api.Models.Prototype
{
    public abstract class FishPrototype
    {
        public int Id { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public int Points { get; set; }
        public double Radius { get; set; }
        public string? Color { get; set; }
        public double MovementSpeed { get; set; }
        public bool HasBeenHooked { get; set; }
        
        public FishMovementBehavior? Movement { get; set; }

        protected FishPrototype()
        {
            Id = new Random().Next(1000, 9999);
        }

        public abstract FishPrototype CloneShallow();

        public abstract FishPrototype CloneDeep();

        public string GetMemoryAddress()
        {
            GCHandle handle = GCHandle.Alloc(this, GCHandleType.Weak);
            string address = $"0x{GCHandle.ToIntPtr(handle).ToInt64():X16}";
            handle.Free();
            return address;
        }

        public string GetMovementAddress()
        {
            if (Movement == null) return "null";
            GCHandle handle = GCHandle.Alloc(Movement, GCHandleType.Weak);
            string address = $"0x{GCHandle.ToIntPtr(handle).ToInt64():X16}";
            handle.Free();
            return address;
        }

        public virtual string GetInfo()
        {
            return $@"
Fish Type:              {this.GetType().Name}
Object Address:         {GetMemoryAddress()}
Movement Address:       {GetMovementAddress()}
ID:                     {Id}
Position:               ({PositionX:F2}, {PositionY:F2})
Points:                 {Points}
Movement:               {Movement?.ToString() ?? "null"}
";
        }
    }

    public class FishMovementBehavior
    {
        public string StrategyType { get; set; }
        public double Speed { get; set; }
        public double DirectionX { get; set; }
        public double DirectionY { get; set; }

        public FishMovementBehavior(string strategyType, double speed, double dirX, double dirY)
        {
            StrategyType = strategyType;
            Speed = speed;
            DirectionX = dirX;
            DirectionY = dirY;
        }

        public FishMovementBehavior DeepClone()
        {
            return new FishMovementBehavior(this.StrategyType, this.Speed, this.DirectionX, this.DirectionY);
        }

        public string GetMemoryAddress()
        {
            GCHandle handle = GCHandle.Alloc(this, GCHandleType.Weak);
            string address = $"0x{GCHandle.ToIntPtr(handle).ToInt64():X16}";
            handle.Free();
            return address;
        }

        public override string ToString()
        {
            return $"[{StrategyType}, Speed={Speed:F2}, Dir=({DirectionX:F2},{DirectionY:F2})]";
        }
    }
}
