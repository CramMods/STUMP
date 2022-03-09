using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;
using CramMods.STUMP.Readers;
using CramMods.NARFI.Skyrim;
using CramMods.STUMP.Types;

namespace CramMods.STUMP
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            // Read Settings
            IReadOnlyList<RaceGroup> raceGroups = RaceGroupsReader.Read(state);
            IReadOnlyList<Variant> variants = VariantsReader.Read(state);
            throw new Exception();
        }
    }
}
