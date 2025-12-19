namespace Api.Models
{
    public class FishSchool : IFishGroup
    {
        private readonly List<Fish> _members = new();
        private IFishMove _schoolMove;

        public FishSchool(IEnumerable<Fish> fishes)
        {
            _members.AddRange(fishes);
            _schoolMove = _members[0].FishMove;
        }

        public void Update(int w, int h)
        {
            // Pick a leader (first fish) just for boundary detection
            var leader = _members[0];

            // Apply the same movement strategy to everyone BEFORE moving
            foreach (var f in _members)
            {
                f.DisableBoundaryStrategyChange = true;
                f.FishMove = _schoolMove;
            }

            // Move everyone (still uses each fish's MoveWithCurrentStrategy,
            // but since FishMove is shared, they go same direction)
            foreach (var f in _members)
                f.UpdatePosition(w, h);

            // If leader hits boundary, change school direction ONCE
            if (leader.PositionX <= 0 || leader.PositionX >= w ||
                leader.PositionY <= 0 || leader.PositionY >= h)
            {
                _schoolMove = leader.GetNewMovementStrategy();
            }
        }


        public IEnumerable<Fish> Flatten() => _members;

        public bool RemoveFish(int fishId)
        {
            var removed = _members.RemoveAll(f => f.Id == fishId) > 0;
            return removed;
        }

        public void TriggerScare()
        {
            foreach (var f in _members)
                f.TriggerScare();
        }

        public bool IsEmpty => _members.Count == 0;
    }
}
