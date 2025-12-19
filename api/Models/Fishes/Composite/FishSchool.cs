namespace Api.Models
{
    public class FishSchool : IFishGroup
    {
        private readonly List<Fish> _members = new();

        public FishSchool(IEnumerable<Fish> fishes)
        {
            _members.AddRange(fishes);
        }

        public void Update(int w, int h)
        {
            foreach (var f in _members)
                f.UpdatePosition(w, h);
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
