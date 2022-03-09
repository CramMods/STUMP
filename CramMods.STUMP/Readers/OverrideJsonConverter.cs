using CramMods.STUMP.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CramMods.STUMP.Readers
{
    public class OverrideJsonConverter : JsonConverter<Override>
    {
        public override Override? ReadJson(JsonReader reader, Type objectType, Override? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) throw new JsonSerializationException("Expected an object");
            Dictionary<string, JToken>? rawDict = serializer.Deserialize<Dictionary<string, JToken>>(reader);
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
