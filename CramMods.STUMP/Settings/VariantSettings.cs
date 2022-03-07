using CramMods.NARFI.Filters;
using Newtonsoft.Json;

namespace CramMods.STUMP.Settings
{
    [JsonConverter(typeof(VariantSettingsJsonConverter))]
    public class VariantSettings
    {
        private string? _name;
        public string? Name { get => _name; set => _name = value; }

        public VariantSettings() { }
        public VariantSettings(string name) : this() => _name = name;

        private VariantType _type = VariantType.Normal;
        public VariantType Type { get => _type; set => _type = value; }

        private VariantSelectMode _select = VariantSelectMode.Multiple;
        public VariantSelectMode Select { get => _select; set => _select = value; }

        private bool _resetName = false;
        public bool SkipName { get => _resetName; set => _resetName = value; }

        private IFilter? _filter;
        public IFilter? Filter { get => _filter; set => _filter = value; }

        private List<Override> _overrides = new();
        public List<Override> Overrides { get => _overrides; set => _overrides = value; }

        private float _weighting = 1.0F;
        public float Weighting { get => _weighting; set => _weighting = value;}

        private List<VariantSettings> _variants = new();
        public List<VariantSettings> Variants { get => _variants; set => _variants = value; }

        public override string ToString()
        {
            string output = (_name == null) ? string.Empty : $"{_name} - ";
            if (_overrides.Count > 0) output += $"O{_overrides.Count} ";
            if (_variants.Count > 0) output += $"V{_variants.Count} ";
            output += $"W{_weighting}";
            return output;
        }

        public List<VariantSettings> Flatten()
        {
            List<VariantSettings> output = new();

            List<VariantSettings> normal = _variants
                .FindAll(v => !v.Type.HasFlag(VariantType.Addon))
                .SelectMany(v => v.Flatten())
                .ToList();

            List<List<VariantSettings>> addonGroups = _variants
                .FindAll(v => v.Type.HasFlag(VariantType.Addon))
                .Select(v => v.Flatten())
                .ToList();

            if (!_type.HasFlag(VariantType.Group)) output.Add(this);

            foreach (VariantSettings item in normal) output.Add(Merge(item));

            foreach (List<VariantSettings> addonGroup in addonGroups)
            {
                foreach (VariantSettings baseItem in output.ToList())
                {
                    foreach (VariantSettings addon in addonGroup)
                    {
                        output.Add(baseItem.AddAddon(addon));
                    }
                }
            }

            return output;
        }

        public VariantSettings Merge(VariantSettings other)
        {
            VariantSettings output = new();
            output.Name = (string.IsNullOrEmpty(_name) || SkipName) ? other.Name : $"{_name}.{other.Name}";
            output.Type = _type & ~VariantType.Group;
            output.Select = VariantSelectMode.Single;
            output.Filter = FilterMerger.Merge(_filter, other.Filter);
            output.Overrides = new List<Override>().Concat(_overrides).Concat(other.Overrides).ToList();
            output.Weighting = _weighting * other.Weighting;
            return output;
        }

        public VariantSettings AddAddon(VariantSettings addon)
        {
            VariantSettings output = Merge(addon);
            output.Name = $"{_name} +{addon.Name}";
            return output;
        }
    }

    public class VariantSettingsJsonConverter : JsonConverter<VariantSettings>
    {
        public override VariantSettings? ReadJson(JsonReader reader, Type objectType, VariantSettings? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!serializer.Converters.Any(c => c is FilterJsonConverter)) serializer.Converters.Add(new FilterJsonConverter());

            if (reader.TokenType != JsonToken.StartObject) throw new JsonSerializationException("Expected an object");
            reader.Read();

            VariantSettings variantSettings = existingValue ?? new();

            while (reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName) throw new JsonSerializationException("Expected a property name");
                string propertyName = (string)reader.Value!;
                reader.Read();

                if (propertyName.Equals("type", StringComparison.InvariantCultureIgnoreCase)) variantSettings.Type = serializer.Deserialize<VariantType>(reader);
                else if (propertyName.Equals("select", StringComparison.InvariantCultureIgnoreCase)) variantSettings.Select = serializer.Deserialize<VariantSelectMode>(reader);
                else if (propertyName.Equals("skipname", StringComparison.InvariantCultureIgnoreCase)) variantSettings.SkipName = serializer.Deserialize<bool>(reader);
                else if (propertyName.Equals("filter", StringComparison.InvariantCultureIgnoreCase)) variantSettings.Filter = serializer.Deserialize<IFilter>(reader);
                else if (propertyName.Equals("overrides", StringComparison.InvariantCultureIgnoreCase)) variantSettings.Overrides = serializer.Deserialize<List<Override>>(reader) ?? new();
                else if (propertyName.Equals("weighting", StringComparison.InvariantCultureIgnoreCase)) variantSettings.Weighting = serializer.Deserialize<float>(reader);
                else if (reader.TokenType == JsonToken.StartObject)
                {
                    VariantSettings? subvariant = (VariantSettings?)ReadJson(reader, objectType, new VariantSettings(propertyName), serializer);
                    if (subvariant != null) variantSettings.Variants.Add(subvariant);
                }
                else throw new NotImplementedException($"Unknown property: {propertyName}");

                reader.Read();
            }

            return variantSettings;
        }

        public override void WriteJson(JsonWriter writer, VariantSettings? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
