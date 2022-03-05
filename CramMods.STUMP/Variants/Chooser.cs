using Mutagen.Bethesda.Skyrim;

namespace CramMods.STUMP.Variants
{
    public static class Chooser
    {
        public static Variant? Choose(List<Variant> variants, INpcGetter npc, NARFI.NARFI narfi)
        {
            List<Variant> valid = variants.FindAll(v => v.Filter.IsMatch(npc, narfi));
            if (valid.Count == 0) return null;

            int maxForceCount = valid.Max(v => v.Filter.GetForceCount());
            valid = valid.FindAll(v => v.Filter.GetForceCount() == maxForceCount);
            if (valid.Count == 0) return null;

            float totalWeight = valid.Sum(v => v.Weighting);
            float random = new Random().NextSingle() * totalWeight;

            float cumulative = 0.0F;
            foreach (Variant variant in valid)
            {
                cumulative += variant.Weighting;
                if (random < cumulative) return variant;
            }

            throw new Exception();
        }
    }
}
