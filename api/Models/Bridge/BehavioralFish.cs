namespace Api.Models.Bridge
{
    public class BehavioralFish
    {
        private IFishBehavior _behaviorImplementation;
        public string FishType { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public BehavioralFish(string fishType, double x, double y, IFishBehavior behavior)
        {
            FishType = fishType;
            X = x;
            Y = y;
            _behaviorImplementation = behavior;
        }

        public void SetBehavior(IFishBehavior behavior)
        {
            Console.WriteLine($"🔄 {FishType} changed behavior from {_behaviorImplementation.Behavior} to {behavior.Behavior}");
            _behaviorImplementation = behavior;
        }

        public IFishBehavior GetBehavior() => _behaviorImplementation;

        public void PerformAction()
        {
            Console.WriteLine($"\n{FishType} at ({X}, {Y}):");
            _behaviorImplementation.Act();
        }

        public void ShowBehaviorStats()
        {
            Console.WriteLine($"{FishType} ({_behaviorImplementation.Behavior}):");
            Console.WriteLine($"  🎯 Hunt Probability: {_behaviorImplementation.GetHuntingProbability() * 100}%");
            Console.WriteLine($"  💨 Escape Probability: {_behaviorImplementation.GetEscapeProbability() * 100}%");
        }
        public string GetCombinationDescription()
        {
            return $"{FishType} with {_behaviorImplementation.Behavior} behavior";
        }
    }
}
