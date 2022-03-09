using CramMods.STUMP.Types;
using System.Collections.Generic;
using System.Linq;

namespace CramMods.STUMP.Helpers
{
    public static class OverrideUtil
    {
        public static List<Override>? Merge(List<Override> overrides1, List<Override> overrides2)
        {
            List<Override> output = overrides1.Select(o => o.Clone()).ToList();

            foreach (Override over2 in overrides2)
            {
                Override? matching = output.FirstOrDefault(o => (o.Type == over2.Type) && (o.Part == over2.Part));
                if (matching == null)
                {
                    matching = new() { Part = over2.Part, Type = over2.Type };
                    output.Add(matching);
                }
                if (matching.Lock) return null;

                matching.Path = over2.Path;
                matching.Lock = over2.Lock;
            }

            return output;
        }
    }
}
