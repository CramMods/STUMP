using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using CramMods.NARFI.Skyrim;
using CramMods.STUMP.Readers;
using CramMods.STUMP.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CramMods.STUMP
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "STUMP.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            // Read Settings
            IReadOnlyList<RaceGroup> raceGroups = RaceGroupsReader.Read(state);
            IReadOnlyList<Variant> variants = VariantsReader.Read(state);


        }
    }
}
