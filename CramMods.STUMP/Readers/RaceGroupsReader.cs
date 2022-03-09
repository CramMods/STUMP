using CramMods.NARFI.Skyrim;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CramMods.STUMP.Readers
{
    public static class RaceGroupsReader
    {
        private static string _fileName = "RaceGroups.json";

        public static IReadOnlyList<RaceGroup> Read(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            string path = Path.Combine(state.ExtraSettingsDataPath, _fileName);
            if (!File.Exists(path)) throw new FileNotFoundException($"Settings file missing: \"{path}\"");

            string content = File.ReadAllText(path);

            Dictionary<string, string[]>? raceNamesDict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(content);
            if (raceNamesDict == null) throw new JsonSerializationException("Invalid settings file contents. Expected to be a dictionary");

            return RaceGroup.FromIdDictionary(raceNamesDict, state.LoadOrder.PriorityOrder.Race().WinningOverrides()).ToList().AsReadOnly();
        }
    }
}
