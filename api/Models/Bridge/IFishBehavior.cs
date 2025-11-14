namespace Api.Models.Bridge
{
    public interface IFishBehavior
    {
        string Behavior { get; }
        void Act();
        double GetHuntingProbability();
        double GetEscapeProbability();
    }

    public class AggressiveBehavior : IFishBehavior
    {
        public string Behavior => "Aggressive";

        public void Act()
        {
            Console.WriteLine("🐠 Acting aggressively - hunting for food!");
        }

        public double GetHuntingProbability() => 0.8; 
        public double GetEscapeProbability() => 0.2;  
    }

    public class PassiveBehavior : IFishBehavior
    {
        public string Behavior => "Passive";

        public void Act()
        {
            Console.WriteLine("Acting passively - exploring calmly...");
        }

        public double GetHuntingProbability() => 0.3; // 30% hunting
        public double GetEscapeProbability() => 0.7;  // 70% escape
    }

    public class TerritorialBehavior : IFishBehavior
    {
        public string Behavior => "Territorial";

        public void Act()
        {
            Console.WriteLine("Acting territorially - defending area!");
        }

        public double GetHuntingProbability() => 0.6; // 60% hunting
        public double GetEscapeProbability() => 0.4;  // 40% escape
    }

    public class NocturnalBehavior : IFishBehavior
    {
        public string Behavior => "Nocturnal";

        public void Act()
        {
            Console.WriteLine(" Acting nocturnally - hunting at night!");
        }

        public double GetHuntingProbability() => 0.75; // 75% hunting
        public double GetEscapeProbability() => 0.25;  // 25% escape
    }
}
