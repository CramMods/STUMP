using CramMods.NARFI.FieldValueGetters;
using CramMods.STUMP.Settings;
using Mutagen.Bethesda.Skyrim;

namespace CramMods.STUMP
{
    public static class VariantUtils
    {
        public static Dictionary<INpcGetter, VariantSettings> MatchVariants(IEnumerable<INpcGetter> npcs, IEnumerable<VariantSettings> variants, IFieldValueGetter fieldValueGetter, MatchVariantsCallback? callback = null)
        {
            Dictionary<INpcGetter, VariantSettings> output = new();
            int total = npcs.Count();
            int current = 0;
            int matchCount = 0;

            foreach (INpcGetter npc in npcs)
            {
                if (callback != null) callback.Invoke(total, current++, matchCount);

                IEnumerable<VariantSettings> matchingVariants = variants.Where(v => v.Filter?.Test(npc, fieldValueGetter) ?? false);
                if (matchingVariants.Count() == 0) continue;

                int maxForce = matchingVariants.Max(v => v.ForceCount);
                matchingVariants = matchingVariants.Where(v => v.ForceCount == maxForce);
                if (matchingVariants.Count() == 0) continue;

                output.Add(npc, SelectRandomVariant(matchingVariants));
                matchCount++;
            }
            if (callback != null) callback.Invoke(total, current, matchCount);

            return output;
        }
        public delegate void MatchVariantsCallback(int total, int complete, int matchCount);

        public static VariantSettings SelectRandomVariant(IEnumerable<VariantSettings> variants)
        {
            float total = variants.Sum(v => v.Weighting);
            float random = new Random().NextSingle() * total;
            float next = 0.0F;

            foreach (VariantSettings variant in variants)
            {
                next += variant.Weighting;
                if (next >= random) return variant;
            }

            throw new Exception("Something went wrong. Shouldn't be possible");
        }
    }
}
