namespace Api.Models.ChainOfResponsibility
{
    /// <summary>
    /// CHAIN OF RESPONSIBILITY - Handler 4
    /// Checks if fish is inside any HazardZone (red circles) or Obstacles
    /// </summary>
    public class ObstacleCheckHandler : CatchAttemptHandler
    {
        public override void Handle(CatchAttemptContext context)
        {
            ProcessRequest(context);
            base.Handle(context);
        }

        protected override void ProcessRequest(CatchAttemptContext context)
        {
            context.AddLog("Handler 4: Checking for hazard zones and obstacles...");

            double fishX = context.Fish.PositionX;
            double fishY = context.Fish.PositionY;

            // CHECK 1: HazardZones (red circles) - Fish inside cannot be caught
            var hazardZones = context.Session.Environment.HazardZones;
            if (hazardZones != null && hazardZones.Count > 0)
            {
                foreach (var zone in hazardZones)
                {
                    if (IsFishInsideHazardZone(fishX, fishY, zone))
                    {
                        context.Fail($"🔴 Fish is inside hazard zone! Cannot catch fish in danger area (Zone at X:{zone.X:F0}, Y:{zone.Y:F0})");
                        return;
                    }
                }
                context.AddLog($"   Checked {hazardZones.Count} hazard zones - fish is safe");
            }

            // CHECK 2: Obstacles (seaweed/rocks)
            var obstacles = context.Session.Environment.Obstacles;
            if (obstacles != null && obstacles.Count > 0)
            {
                foreach (var obstacle in obstacles)
                {
                    if (IsFishInsideObstacle(fishX, fishY, obstacle))
                    {
                        context.Fail($"🌿 Fish is hiding in obstacle! (Obstacle at X:{obstacle.PositionX:F0}, Y:{obstacle.PositionY:F0})");
                        return;
                    }
                }
            }

            context.AddLog("✅ Obstacle/hazard check passed");
        }

        private bool IsFishInsideHazardZone(double fishX, double fishY, HazardZone zone)
        {
            // HazardZone is a circle - check if fish is within radius
            double dx = fishX - zone.X;
            double dy = fishY - zone.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            
            if (distance <= zone.Radius)
            {
                Console.WriteLine($"🔴 Fish at ({fishX:F0}, {fishY:F0}) is INSIDE hazard zone at ({zone.X:F0}, {zone.Y:F0}) radius {zone.Radius:F0}");
                return true;
            }
            
            return false;
        }

        private bool IsFishInsideObstacle(double fishX, double fishY, Obstacle obstacle)
        {
            // Check if fish position is inside the obstacle's rectangular area
            double obstacleLeft = obstacle.PositionX - (obstacle.Width / 2.0);
            double obstacleRight = obstacle.PositionX + (obstacle.Width / 2.0);
            double obstacleTop = obstacle.PositionY;
            double obstacleBottom = obstacle.PositionY + obstacle.Height;
            
            bool insideX = fishX >= obstacleLeft && fishX <= obstacleRight;
            bool insideY = fishY >= obstacleTop && fishY <= obstacleBottom;
            
            if (insideX && insideY)
            {
                Console.WriteLine($"🚧 Fish at ({fishX:F0}, {fishY:F0}) is INSIDE obstacle at ({obstacle.PositionX:F0}, {obstacle.PositionY:F0})");
                return true;
            }
            
            return false;
        }

        private bool IsObstacleBlocking(double x1, double y1, double x2, double y2, Obstacle obstacle)
        {
            // OLD LOGIC - kept for reference but not used
            double obstacleX = obstacle.PositionX;
            double obstacleY = obstacle.PositionY;
            double obstacleRadius = obstacle.Width / 2;

            // Calculate distance from obstacle center to line segment
            double distance = DistanceFromPointToLineSegment(x1, y1, x2, y2, obstacleX, obstacleY);
            
            return distance < obstacleRadius + 10; // 10 is tolerance
        }

        private double DistanceFromPointToLineSegment(double x1, double y1, double x2, double y2, double px, double py)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            
            if (dx == 0 && dy == 0)
            {
                // Line segment is a point
                return Math.Sqrt(Math.Pow(px - x1, 2) + Math.Pow(py - y1, 2));
            }

            double t = Math.Max(0, Math.Min(1, ((px - x1) * dx + (py - y1) * dy) / (dx * dx + dy * dy)));
            double nearestX = x1 + t * dx;
            double nearestY = y1 + t * dy;

            return Math.Sqrt(Math.Pow(px - nearestX, 2) + Math.Pow(py - nearestY, 2));
        }
    }
}
