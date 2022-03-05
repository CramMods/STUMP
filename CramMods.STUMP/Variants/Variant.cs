using CramMods.NARFI.Filters;

namespace CramMods.STUMP.Variants
{
    public class Variant
    {
        private string _name = string.Empty;
        public string Name { get => _name; set => _name = value; }

        private IFilter _filter = new NullFilter();
        public IFilter Filter { get => _filter; set => _filter = value; }

        private List<IOverride> _override = new();
        public List<IOverride> Overrides { get => _override; set => _override = value; }

        private float _weighting = 1.0F;
        public float Weighting { get => _weighting; set => _weighting = value; }

        public override string ToString() => _name;
    }
}
