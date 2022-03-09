using CramMods.STUMP.Types;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace CramMods.STUMP.Readers
{
    public static class VariantsReader
    {
        private static string _fileName = "Variants.json";

        public static IReadOnlyList<Variant> Read(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            string path = Path.Combine(state.ExtraSettingsDataPath, _fileName);
            if (!File.Exists(path)) throw new FileNotFoundException($"Settings file missing: \"{path}\"");

            string content = File.ReadAllText(path);

            JsonSerializerSettings settings = new();
            settings.Converters.Add(new FilterJsonConverter());
            settings.Converters.Add(new OverrideJsonConverter());
            settings.Converters.Add(new VariantJsonConverter());

            Variant? root = JsonConvert.DeserializeObject<Variant>(content, settings);
            if (root == null) throw new JsonSerializationException("Invalid settings file contents.");

            return root.Flatten();
        }
    }
}
