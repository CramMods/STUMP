using CramMods.NARFI.Filters;
using Newtonsoft.Json;

using GroupFilterOperator = CramMods.NARFI.Filters.GroupFilterOperator;

namespace CramMods.STUMP.Variants
{

    [JsonConverter(typeof(VariantBuilderJsonConverter))]
    public class VariantBuilder
    {
        private string _name = string.Empty;
        public string Name { get => _name; set => _name = value; }

        private bool _resetName = false;
        public bool ResetName { get => _resetName; set => _resetName = value; }

        private VariantBuilderType _type = VariantBuilderType.Normal;
        public VariantBuilderType Type { get => _type; set => _type = value; }

        private SelectMode _select = SelectMode.Multiple;
        public SelectMode Select { get => _select; set => _select = value; }

        private float _weighting = 1.0F;
        public float Weighting { get => _weighting; set => _weighting = value; }

        private IFilter _filter = new NullFilter();
        public IFilter Filter { get => _filter; set => _filter = value; }

        private List<IOverride> _overrides = new();
        public List<IOverride> Overrides { get => _overrides; set => _overrides = value; }

        private List<VariantBuilder> _variants = new();
        public List<VariantBuilder> Variants { get => _variants; set => _variants = value; }

        public override string ToString() => $"{_name} [{_variants.Count}]";

        public List<VariantBuilder> Flatten()
        {
            List<VariantBuilder> outputVariants = new();

            if ( ((_overrides.Count > 0) || (_variants.Count == 0)) && !_type.HasFlag(VariantBuilderType.Group)) outputVariants.Add(this);

            List<VariantBuilder> toMerge = _variants
                .Where(v => !v.Type.HasFlag(VariantBuilderType.Addon))
                .SelectMany(v => v.Flatten())
                .ToList();

            List<List<VariantBuilder>> toAdd = _variants
                .Where(v => v.Type.HasFlag(VariantBuilderType.Addon))
                .Select(v => v.Flatten())
                .ToList();

            outputVariants.AddRange(toMerge.Select(m => Merge(m)));
            
            foreach (List<VariantBuilder> addonGroup in toAdd)
            {
                if (addonGroup.Count == 0) continue;
                List<VariantBuilder> bases = outputVariants.ToList();

                if ((addonGroup[0].Select == SelectMode.Single) || (addonGroup.Count == 1))
                {
                    foreach (VariantBuilder b in bases)
                    {
                        foreach (VariantBuilder addon in addonGroup) outputVariants.Add(b.Add(addon));
                    }
                }
            }

            return outputVariants;
        }

        public List<Variant> Simplify()
        {
            List<VariantBuilder> variantBuilders = Flatten();
            return variantBuilders
                .ConvertAll(vb => {
                    return new Variant()
                    {
                        Name = vb.Name,
                        Filter = vb.Filter,
                        Overrides = vb.Overrides,
                        Weighting = vb.Weighting,
                    };
                });
        }

        public VariantBuilder Merge(VariantBuilder other)
        {
            VariantBuilder output = new();

            output.Name = (string.IsNullOrEmpty(_name) || _resetName) ? other.Name : $"{_name}.{other.Name}";
            output.Type = _type & ~VariantBuilderType.Group;
            output.Select = _select;

            output.Filter = _filter.Merge(other.Filter);

            output.Overrides.AddRange(_overrides);
            output.Overrides.AddRange(other.Overrides);

            output.Weighting = _weighting * other.Weighting;
            output.ResetName = _resetName || other.ResetName;

            return output;
        }

        public VariantBuilder Add(VariantBuilder other)
        {
            VariantBuilder output = Merge(other);
            output.Name = $"{_name} +{other.Name}";
            return output;
        }
    }

    [Flags]
    public enum VariantBuilderType { Normal = 0, Addon = 1, Group = 2 }

    public enum SelectMode { Multiple = 0, Many = Multiple, Single, One = Single }


    public class VariantBuilderJsonConverter : JsonConverter<VariantBuilder>
    {
        public override VariantBuilder? ReadJson(JsonReader reader, Type objectType, VariantBuilder? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!serializer.Converters.Any(c => c is FilterJsonConverter)) serializer.Converters.Add(new FilterJsonConverter());

            if (reader.TokenType != JsonToken.StartObject) throw new JsonSerializationException("Expected an object");
            reader.Read();

            VariantBuilder builder = existingValue ?? new();

            while (reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName) throw new JsonSerializationException("Expected a property name");
                string itemName = reader.Value!.ToString()!;
                reader.Read();

                if (itemName.Equals("Type", StringComparison.InvariantCultureIgnoreCase)) builder.Type = serializer.Deserialize<VariantBuilderType>(reader);
                else if (itemName.Equals("Select", StringComparison.InvariantCultureIgnoreCase)) builder.Select = serializer.Deserialize<SelectMode>(reader);
                else if (itemName.Equals("Weighting", StringComparison.InvariantCultureIgnoreCase)) builder.Weighting = serializer.Deserialize<float>(reader)!;
                else if (itemName.Equals("Overrides", StringComparison.InvariantCultureIgnoreCase)) builder.Overrides = serializer.Deserialize<List<IOverride>>(reader)!;
                else if (itemName.Equals("Filter", StringComparison.InvariantCultureIgnoreCase)) builder.Filter = serializer.Deserialize<IFilter>(reader)!;
                else if (itemName.Equals("ResetName", StringComparison.InvariantCultureIgnoreCase)) builder.ResetName = serializer.Deserialize<bool>(reader)!;
                else if (reader.TokenType == JsonToken.StartObject) builder.Variants.Add((VariantBuilder)ReadJson(reader, objectType, new VariantBuilder() { Name = itemName }, serializer)!);
                else throw new NotImplementedException(itemName);

                reader.Read();
            }

            return builder;
        }

        public override void WriteJson(JsonWriter writer, VariantBuilder? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
