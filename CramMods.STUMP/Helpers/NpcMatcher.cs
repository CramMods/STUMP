using CramMods.NARFI.FieldValueGetters;
using CramMods.STUMP.Types;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CramMods.STUMP.Helpers
{
    public class NpcMatcher
    {
        private IFieldValueGetter _getter;

        public NpcMatcher(IFieldValueGetter getter) => _getter = getter;

        public Dictionary<INpcGetter, Variant> MatchAllNpcs(IGameEnvironmentState<ISkyrimMod, ISkyrimModGetter> state, IEnumerable<Variant> variants, NpcMatcherCallback? callback = null) =>
            MatchNpcs(state.LoadOrder.PriorityOrder.Npc().WinningOverrides(), variants, callback);

        public Dictionary<INpcGetter, Variant> MatchNpcs(IEnumerable<INpcGetter> npcs, IEnumerable<Variant> variants, NpcMatcherCallback? callback = null)
        {
            int total = npcs.Count();
            int processed = 0;
            int matched = 0;

            Dictionary<INpcGetter, Variant> result = new();

            foreach (INpcGetter npc in npcs)
            {
                if (callback != null) callback.Invoke(total, processed, matched, npc);

                Variant? match = MatchNpc(npc, variants);
                processed++;
                if (match == null) continue;
                matched++;
                result.Add(npc, match);
            }

            if (callback != null) callback.Invoke(total, processed, matched, null);

            return result;
        }

        public Variant? MatchNpc(INpcGetter npc, IEnumerable<Variant> variants)
        {
            List<Variant> matching = variants.ToList().FindAll(v => v.Filter?.Test(npc, _getter) ?? false);
            if (matching.Count == 0) return null;

            int maxForced = matching.Max(v => v.ForceCount);
            matching = matching.FindAll(v => v.ForceCount == maxForced);
            if (matching.Count == 0) return null;

            return SelectRandom(matching);
        }

        private Variant SelectRandom(IEnumerable<Variant> variants)
        {
            float totalWeighting = variants.Sum(v => v.Weighting);
            float random = new Random().NextSingle() * totalWeighting;
            float next = 0.0F;
            foreach (Variant variant in variants)
            {
                next += variant.Weighting;
                if (next >= random) return variant;
            }
            throw new Exception("Impossible error");
        }
    }

    public delegate void NpcMatcherCallback(int total, int processed, int matched, INpcGetter? current);
}
