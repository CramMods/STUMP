using CramMods.NARFI.Filters;

namespace CramMods.STUMP.Variants
{
    public static class FilterExtensions
    {
        public static bool GetForce(this IFilter filter) => filter.Data.ContainsKey("Force") ? (bool)filter.Data["Force"] : false;
        public static void SetForce(this IFilter filter, bool value) => filter.Data["Force"] = value;

        public static int GetForceCount(this IFilter filter)
        {
            int count = filter.GetForce() ? 1 : 0;
            if (filter is GroupFilter) count += ((GroupFilter)filter).Filters.Select(f => f.GetForceCount()).Sum();
            return count;
        }
        
        public static IFilter Merge(this IFilter filter1, IFilter filter2)
        {
            IFilter? output;

            if (filter1 is NullFilter) output = filter2;
            else if (filter2 is NullFilter) output = filter1;
            else
            {
                output = new GroupFilter(GroupFilterOperator.AND);
                GroupFilter outGroup = (GroupFilter)output;

                if ((filter1 is GroupFilter) && (((GroupFilter)filter1).Operator == GroupFilterOperator.AND)) outGroup.Filters.AddRange(((GroupFilter)filter1).Filters);
                else outGroup.Filters.Add(filter1);

                if ((filter2 is GroupFilter) && (((GroupFilter)filter2).Operator == GroupFilterOperator.AND)) outGroup.Filters.AddRange(((GroupFilter)filter2).Filters);
                else outGroup.Filters.Add(filter2);
            }

            if (output == null) throw new Exception();

            return output;
        }
    }
}
