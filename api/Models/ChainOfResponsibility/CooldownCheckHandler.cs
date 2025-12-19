namespace Api.Models.ChainOfResponsibility
{
    /// <summary>
    /// CHAIN OF RESPONSIBILITY - Handler 1
    /// Checks if the fishing rod is on cooldown
    /// Uses Player's LastCatchTime property
    /// </summary>
    public class CooldownCheckHandler : CatchAttemptHandler
    {
        private readonly TimeSpan _cooldownDuration = TimeSpan.FromSeconds(0.5); // 0.5 second cooldown

        public override void Handle(CatchAttemptContext context)
        {
            ProcessRequest(context);
            base.Handle(context);
        }

        protected override void ProcessRequest(CatchAttemptContext context)
        {
            context.AddLog("Handler 1: Checking fishing rod cooldown...");

            var player = context.Player;
            var timeSinceLastCatch = DateTime.UtcNow - player.LastCatchTime;
            
            if (timeSinceLastCatch < _cooldownDuration)
            {
                var remainingCooldown = _cooldownDuration - timeSinceLastCatch;
                context.Fail($"⏱️ Fishing rod on cooldown! Wait {remainingCooldown.TotalSeconds:F1}s");
                return;
            }

            // Update last catch time
            player.LastCatchTime = DateTime.UtcNow;
            context.AddLog($"✅ Cooldown check passed (last catch: {timeSinceLastCatch.TotalSeconds:F1}s ago)");
        }
    }
}
