using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using CramMods.NARFI.Skyrim;
using CramMods.STUMP.Readers;
using CramMods.STUMP.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using CramMods.STUMP.Helpers;
using CramMods.NARFI;
using System;

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

            // Prepare NARFI
            NARFI.NARFI narfi = new(StateUtil.PatcherToEnvironment(state));
            SkyrimPlugin skyrimPlugin = new SkyrimPlugin();
            skyrimPlugin.SetRaceGroups(new(raceGroups));
            narfi.RegisterPlugin(skyrimPlugin);

            // Match NPCs
            NpcMatcher npcMatcher = new(narfi);
            Dictionary<INpcGetter, Variant> matches = npcMatcher.MatchAllNpcs(StateUtil.PatcherToEnvironment(state), variants, DisplayNpcMatchMessage);

        }

        private static void DisplayNpcMatchMessage(int total, int processed, int matched, INpcGetter? current)
        {
            int totalWidth = Console.BufferWidth;
            int percent = (int)((float)processed / total * 100);
            int totalLength = total.ToString().Length;

            if (current != null)
            {
                Console.Write($"[ {percent,3}% ]  [ {processed.ToString().PadLeft(totalLength)} / {total} ]  Matching NPC: {current.EditorID ?? current.FormKey.ToString()}".PadRight(totalWidth));
                Console.Write("\r");
            }
            else
            {
                Console.WriteLine($"[ {percent,3}% ]  [ {processed.ToString().PadLeft(totalLength)} / {total} ]  Found Matches for {matched} NPCs".PadRight(totalWidth));
                Console.Write("\r");
            }
        }
    }
}
