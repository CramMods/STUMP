using CramMods.NARFI.FieldValueGetters;
using CramMods.NARFI.Filters;
using Mutagen.Bethesda.Plugins.Records;

namespace CramMods.STUMP.Variants
{
    public class NullFilter : IFilter
    {
        public bool IsMatch(IMajorRecordGetter record, IFieldValueGetter fieldGetter) => true;
        public Dictionary<string, object> Data => new();
    }
}
