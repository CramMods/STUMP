using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CramMods.STUMP.Settings
{
    [JsonConverter(typeof(OverrideJsonConverter))]
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

    public class OverrideJsonConverter : JsonConverter<Override>
    {
        public override Override? ReadJson(JsonReader reader, Type objectType, Override? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) throw new JsonSerializationException("Expected an object");
            IDictionary<string, JToken>? rawDict = serializer.Deserialize<Dictionary<string, JToken>>(reader);
            if (rawDict == null) throw new JsonSerializationException("Expected a dictionary");

            Dictionary<string, JToken> items = new(rawDict, StringComparer.InvariantCultureIgnoreCase);

            Override o = new();
            if (items.ContainsKey("part")) o.Part = items["part"].ToObject<OverridePart>(serializer);
            if (items.ContainsKey("type")) o.Type = items["type"].ToObject<OverrideType>(serializer);
            if (items.ContainsKey("lock")) o.Lock = items["lock"].ToObject<bool>(serializer);
            if (items.ContainsKey("path")) o.Path = items["path"].ToObject<string>(serializer) ?? throw new JsonSerializationException("Expected path to be a string");

            return o;
        }

        public override void WriteJson(JsonWriter writer, Override? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}