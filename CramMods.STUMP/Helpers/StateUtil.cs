using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace CramMods.STUMP.Helpers
{
    public static class StateUtil
    {
        public static IGameEnvironmentState<ISkyrimMod, ISkyrimModGetter> PatcherToEnvironment(IPatcherState<ISkyrimMod, ISkyrimModGetter> patcherState) => new GameEnvironmentState<ISkyrimMod, ISkyrimModGetter>(
                patcherState.GameRelease,
                patcherState.DataFolderPath,
                patcherState.LoadOrderFilePath,
                null,
                patcherState.LoadOrder,
                patcherState.LinkCache);
    }
}
