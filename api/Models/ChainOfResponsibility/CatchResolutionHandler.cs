namespace Api.Models.ChainOfResponsibility
{
    /// <summary>
    /// CHAIN OF RESPONSIBILITY - Handler 5 (Final)
    /// Resolves the catch attempt if all validations passed
    /// </summary>
    public class CatchResolutionHandler : CatchAttemptHandler
    {
        public override void Handle(CatchAttemptContext context)
        {
            ProcessRequest(context);
            // No next handler - this is the end of the chain
        }

        protected override void ProcessRequest(CatchAttemptContext context)
        {
            if (!context.IsValid)
            {
                context.AddLog("❌ Cannot resolve catch - validation failed");
                return;
            }

            context.AddLog("Handler 5: Resolving catch attempt...");

            // Mark fish as hooked
            context.Fish.HasBeenHooked = true;

            // Update player score
            int pointsEarned = context.Fish.Points;
            context.Player.Score += pointsEarned;
            context.Player.FishesPulledIn++;

            // Log to session
            context.AddLog($"✅ CATCH SUCCESSFUL! Player '{context.Player.Name}' caught {context.Fish.Color} fish!");
            context.AddLog($"   Points earned: {pointsEarned}");
            context.AddLog($"   New score: {context.Player.Score}");
            context.AddLog($"   Total fish caught: {context.Player.FishesPulledIn}");

            Console.WriteLine($"\n🎣 SUCCESS! {context.Player.Name} caught a {context.Fish.Color} fish worth {pointsEarned} points!");
        }
    }
}
