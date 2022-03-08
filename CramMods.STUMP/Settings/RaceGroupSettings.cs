using CramMods.NARFI.Skyrim;
using Mutagen.Bethesda.Skyrim;
using Newtonsoft.Json;

namespace CramMods.STUMP.Settings
{
    public class RaceGroupSettings : Dictionary<string, string[]>
    {
        public IEnumerable<RaceGroup> Flatten(List<IRaceGetter> allRaces) => RaceGroup.FromIdDictionary(this, allRaces);
    }
}
