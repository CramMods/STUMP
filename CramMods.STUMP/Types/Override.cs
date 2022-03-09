namespace CramMods.STUMP.Types
{
    public class Override
    {
        private OverridePart _part = OverridePart.Body;
        public OverridePart Part { get => _part; set => _part = value; }

        private OverrideType _type = OverrideType.Diffuse;
        public OverrideType Type { get => _type; set => _type = value; }

        private string _path = string.Empty;
        public string Path { get => _path; set => _path = value; }

        private bool _lock = false;
        public bool Lock { get => _lock; set => _lock = value; }

        public override string ToString() => $"{_part} {_type} {_path}";

        public Override() { }
        public Override(Override existing)
        {
            _part = existing.Part;
            _type = existing.Type;
            _path = existing.Path;
            _lock = existing.Lock;
        }

        public Override Clone() => new(this);
    }
}
