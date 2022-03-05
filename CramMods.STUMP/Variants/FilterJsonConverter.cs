using CramMods.NARFI.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CramMods.STUMP.Variants
{
    public class FilterJsonConverter : JsonConverter<IFilter>
    {
        public override IFilter? ReadJson(JsonReader reader, Type objectType, IFilter? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                reader.Skip();
                return new NullFilter();
            }

            if (reader.TokenType != JsonToken.StartObject) throw new JsonSerializationException("Expected an object");
            Dictionary<string, object>? items = serializer.Deserialize<Dictionary<string, object>>(reader);
            if (items == null) throw new JsonSerializationException("Expected a dictionary");
            items = new(items, StringComparer.InvariantCultureIgnoreCase);

            IFilter? filter = null;

            if (items.ContainsKey("Value"))
            {
                Type valueType = items["Value"].GetType();
                Type filterType = typeof(FieldFilter<>).MakeGenericType(valueType);

                IFieldFilter? fieldFilter = (IFieldFilter?)Activator.CreateInstance(filterType);
                if (fieldFilter == null) throw new JsonSerializationException("Unable to create instance");

                fieldFilter.RawValue = items["Value"];

                if (items.ContainsKey("Field")) fieldFilter.FieldPath = new((string)items["Field"]);
                if (items.ContainsKey("Operator")) fieldFilter.Operator = Enum.Parse<FieldFilterOperator>((string)items["Operator"]);

                filter = fieldFilter;
            }
            else if (items.ContainsKey("Filters"))
            {
                GroupFilter groupFilter = new();
                if (items.ContainsKey("Operator")) groupFilter.Operator = Enum.Parse<GroupFilterOperator>((string)items["Operator"]);

                if (!items["Filters"].GetType().IsAssignableTo(typeof(JArray))) throw new JsonSerializationException("Expected an array");
                foreach (JToken token in (JArray)items["Filters"])
                {
                    IFilter? subfilter = token.ToObject<IFilter>(serializer);
                    if ((subfilter != null) && (subfilter is not NullFilter)) groupFilter.Filters.Add(subfilter);
                }

                filter = groupFilter;
            }

            if (filter == null) throw new NotImplementedException();

            if (items.ContainsKey("Force")) filter.SetForce((bool)items["Force"]);

            return filter;
        }

        public override void WriteJson(JsonWriter writer, IFilter? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
