using CramMods.NARFI.Fields;
using CramMods.NARFI.FieldValues;
using CramMods.NARFI.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CramMods.STUMP.Settings
{
    public class FilterJsonConverter : JsonConverter<IFilter>
    {
        public override IFilter? ReadJson(JsonReader reader, Type objectType, IFilter? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) throw new JsonSerializationException("Expected an object");
            IDictionary<string, JToken>? rawDict = serializer.Deserialize<Dictionary<string, JToken>>(reader);
            if (rawDict == null) throw new JsonSerializationException("Expected a dictionary");

            Dictionary<string, JToken> items = new(rawDict, StringComparer.InvariantCultureIgnoreCase);


            IFilter? filter = null;

            if (items.ContainsKey("filters"))
            {
                // Group Filter
                GroupFilterOperator op = GroupFilterOperator.OR;
                if (items.ContainsKey("operator")) op = items["operator"].ToObject<GroupFilterOperator>(serializer);

                List<IFilter> subfilters = new();
                if (items["filters"].Type == JTokenType.Array)
                {
                    foreach (JToken sft in (JArray)items["filters"])
                    {
                        IFilter? sf = sft.ToObject<IFilter>(serializer);
                        if (sf != null) subfilters.Add(sf);
                    }
                }

                GroupFilter groupFilter = new GroupFilter(op, subfilters);
                filter = groupFilter;
            }

            if (items.ContainsKey("field"))
            {
                // Field Filter
                string? fieldString = items["field"].ToObject<string>(serializer);
                if (fieldString == null) throw new JsonSerializationException("Expected field name to be a string");
                FieldPath fieldPath = new(fieldString);

                ComparisonOperator op = ComparisonOperator.EQ;
                if (items.ContainsKey("operator")) op = items["operator"].ToObject<ComparisonOperator>(serializer);

                if (!items.ContainsKey("value")) throw new JsonSerializationException("FieldFilter missing value");
                JTokenType valueType = items["value"].Type;

                Type filterType = valueType switch 
                {
                    JTokenType.String => typeof(string),
                    JTokenType.Boolean => typeof(bool),
                    JTokenType.Integer => typeof(int),
                    JTokenType.Float => typeof(float),
                    _ => throw new JsonSerializationException($"Unknown FieldFilter value type: {valueType}")
                };

                object? value = items["value"].ToObject(filterType, serializer);
                if (value == null) throw new JsonSerializationException("Unable to set value type");

                Type returnType = typeof(FieldFilter<>).MakeGenericType(filterType);
                IFieldFilter? fieldFilter = (IFieldFilter?)Activator.CreateInstance(returnType, fieldPath, op, value);
                if (fieldFilter == null) throw new Exception("Unable to create instance");

                filter = fieldFilter;
            }

            if (filter == null) throw new Exception("Unable to parse filter");

            bool force = false;
            if (items.ContainsKey("force")) force = items["force"].ToObject<bool>(serializer);
            filter.Extensions["Force"] = force;
            
            return filter;
        }

        public override void WriteJson(JsonWriter writer, IFilter? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}