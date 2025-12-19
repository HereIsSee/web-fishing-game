namespace Api.Models.ChainOfResponsibility
{
    /// <summary>
    /// CHAIN OF RESPONSIBILITY - Handler 3
    /// Checks if the fish is within catchable range
    /// </summary>
    public class RangeCheckHandler : CatchAttemptHandler
    {
        private const double MAX_CATCH_RANGE = 100.0;

        public override void Handle(CatchAttemptContext context)
        {
            ProcessRequest(context);
            base.Handle(context);
        }

        protected override void ProcessRequest(CatchAttemptContext context)
        {
            context.AddLog("Handler 3: Checking fish range...");

            double fishX = context.Fish.PositionX;
            double fishY = context.Fish.PositionY;
            double rodX = context.Player.FishingRod.PositionX;
            double rodY = context.Player.FishingRod.PositionY;

            double distance = Math.Sqrt(
                Math.Pow(fishX - rodX, 2) + 
                Math.Pow(fishY - rodY, 2)
            );

            if (distance > MAX_CATCH_RANGE)
            {
                context.Fail($"Fish is too far away! Distance: {distance:F1}, Max range: {MAX_CATCH_RANGE}");
                return;
            }

            context.AddLog($"✅ Range check passed (Distance: {distance:F1})");
        }
    }
}
