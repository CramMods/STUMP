using CramMods.NARFI.Filters;

namespace CramMods.STUMP.Settings
{
    public static class FilterMerger
    {
        public static IFilter? Merge(IFilter? filter1, IFilter? filter2)
        {
            if (filter1 == null) return filter2;
            if (filter2 == null) return filter1;

            GroupFilter output = new(GroupFilterOperator.AND);
            
            if ((filter1 is GroupFilter) && (((GroupFilter)filter1).Operator == GroupFilterOperator.AND)) output.Filters.AddRange(((GroupFilter)filter1).Filters);
            else output.Filters.Add(filter1);

            if ((filter2 is GroupFilter) && (((GroupFilter)filter2).Operator == GroupFilterOperator.AND)) output.Filters.AddRange(((GroupFilter)filter2).Filters);
            else output.Filters.Add(filter2);

            return output;
        }
    }
}