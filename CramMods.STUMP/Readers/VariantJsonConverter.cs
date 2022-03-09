using CramMods.NARFI.Filters;
using CramMods.STUMP.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CramMods.STUMP.Readers
{
    public class VariantJsonConverter : JsonConverter<Variant>
    {
        public override Variant? ReadJson(JsonReader reader, Type objectType, Variant? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) throw new JsonSerializationException("Expected an object");
            reader.Read();

            Variant variant = existingValue ?? new();

            while (reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName) throw new JsonSerializationException("Expected a property name");
                string propertyName = (string)reader.Value!;
                reader.Read();

                if (propertyName.Equals("type", StringComparison.InvariantCultureIgnoreCase)) variant.Type = serializer.Deserialize<VariantType>(reader);
                else if (propertyName.Equals("select", StringComparison.InvariantCultureIgnoreCase)) variant.Select = serializer.Deserialize<VariantSelectMode>(reader);
                else if (propertyName.Equals("skipname", StringComparison.InvariantCultureIgnoreCase)) variant.SkipName = serializer.Deserialize<bool>(reader);
                else if (propertyName.Equals("filter", StringComparison.InvariantCultureIgnoreCase)) variant.Filter = serializer.Deserialize<IFilter>(reader);
                else if (propertyName.Equals("overrides", StringComparison.InvariantCultureIgnoreCase)) variant.Overrides = serializer.Deserialize<List<Override>>(reader) ?? new();
                else if (propertyName.Equals("weighting", StringComparison.InvariantCultureIgnoreCase)) variant.Weighting = serializer.Deserialize<float>(reader);
                else if (reader.TokenType == JsonToken.StartObject)
                {
                    Variant? subvariant = (Variant?)ReadJson(reader, objectType, new Variant(propertyName), serializer);
                    if (subvariant != null) variant.Variants.Add(subvariant);
                }
                else throw new NotImplementedException($"Unknown property: {propertyName}");

                reader.Read();
            }

            return variant;
        }

        public override void WriteJson(JsonWriter writer, Variant? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
