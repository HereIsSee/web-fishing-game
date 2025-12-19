namespace Api.Models
{
    public class FishLeaf : IFishGroup
    {
        private Fish? _fish;

        public FishLeaf(Fish fish) => _fish = fish;

        public void Update(int w, int h)
        {
            _fish?.UpdatePosition(w, h);
        }

        public IEnumerable<Fish> Flatten()
        {
            if (_fish != null) yield return _fish;
        }

        public bool RemoveFish(int fishId)
        {
            if (_fish != null && _fish.Id == fishId)
            {
                _fish = null;
                return true;
            }
            return false;
        }

        public void TriggerScare()
        {
            _fish?.TriggerScare();
        }
    }
}
