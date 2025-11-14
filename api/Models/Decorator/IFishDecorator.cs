namespace Api.Models.Decorator
{
    public interface IFishDecorator
    {
        string DecoratorType { get; }
        double GetPointsMultiplier();
        double GetSlowdownPercentage();
        int GetSlowdownDurationSeconds();
        bool CausesFreeze();
        int GetFreezeDurationSeconds();
        double GetPointsPenalty();
    }

    public class NormalFishDecorator : IFishDecorator
    {
        public string DecoratorType => "Normal";
        public double GetPointsMultiplier() => 1.0;      // Normal points
        public double GetSlowdownPercentage() => 0.0;    // No slowdown
        public int GetSlowdownDurationSeconds() => 0;    // No duration
        public bool CausesFreeze() => false;             // No freeze
        public int GetFreezeDurationSeconds() => 0;      // No freeze duration
        public double GetPointsPenalty() => 0.0;         // No penalty
    }

    public class WeightedFishDecorator : IFishDecorator
    {
        public string DecoratorType => "Weighted";
        public double GetPointsMultiplier() => 1.5;      // 1.5x points
        public double GetSlowdownPercentage() => 0.5;    // 50% slower
        public int GetSlowdownDurationSeconds() => 2;    // 2 seconds
        public bool CausesFreeze() => false;             // No freeze
        public int GetFreezeDurationSeconds() => 0;      // No freeze duration
        public double GetPointsPenalty() => 0.0;         // No penalty
    }

    public class PoisonedFishDecorator : IFishDecorator
    {
        public string DecoratorType => "Poisoned";
        public double GetPointsMultiplier() => 1.0;      // Normal points (but will be negated)
        public double GetSlowdownPercentage() => 0.0;    // No slowdown
        public int GetSlowdownDurationSeconds() => 0;    // No duration
        public bool CausesFreeze() => true;              // Causes freeze
        public int GetFreezeDurationSeconds() => 3;      // 3 seconds
        public double GetPointsPenalty() => 1.0;         // Full deduction of fish points
    }
}
