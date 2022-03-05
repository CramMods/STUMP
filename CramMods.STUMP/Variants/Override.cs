using Newtonsoft.Json;

namespace CramMods.STUMP.Variants
{
    [JsonConverter(typeof(OverrideJsonConverter))]
    public interface IOverride { }

    public class TextureOverride : IOverride
    {
        private OverridePart _part = OverridePart.Body;
        public OverridePart Part { get => _part; set => _part = value; }    

        private OverrideType _type = OverrideType.Diffuse;
        public OverrideType Type { get => _type; set => _type = value; }
        
        private string _path = string.Empty;
        public string Path { get => _path; set => _path = value; }

        public override string ToString() => $"{_part} {_type} {_path}";
    }


    public enum OverridePart { Body = 0, Head, Hands, Feet, Extra }
    public enum OverrideType { Diffuse = 0, Color = Diffuse, Normal, Bump = Normal, Specular, Detail, Subsurface = Detail, Scatter = Detail }


    public class OverrideJsonConverter : JsonConverter<IOverride>
    {
        public override IOverride? ReadJson(JsonReader reader, Type objectType, IOverride? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) throw new JsonSerializationException("Expected an object");

            Dictionary<string, object>? items = serializer.Deserialize<Dictionary<string, object>>(reader);
            if (items == null) throw new JsonSerializationException("Expected a dictionary");
            items = new(items, StringComparer.InvariantCultureIgnoreCase);

            if (items.ContainsKey("Path"))
            {
                // Texture
                TextureOverride o = new();
                o.Path = items["Path"]!.ToString()!;

                if (items.ContainsKey("Type")) o.Type = Enum.Parse<OverrideType>(items["Type"]!.ToString()!);
                if (items.ContainsKey("Part")) o.Part = Enum.Parse<OverridePart>(items["Part"]!.ToString()!);

                return o;
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, IOverride? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
