using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;

namespace AcceleratorThings
{
    [HarmonyPatch(typeof(SavedGame), nameof(SavedGame.Push), typeof(GameModel))]
    internal static class FixPediaThingPatch // stolen from mSRML because lazy
    {
        public static void Prefix(SavedGame __instance)
        {
            if (!__instance.pediaEntryLookup.ContainsKey(EntryPoint.vacceleratorDef.PediaEntry.PersistenceId))
                __instance.pediaEntryLookup.Add(EntryPoint.vacceleratorDef.PediaEntry.PersistenceId, EntryPoint.vacceleratorDef.PediaEntry);
            if (!__instance.pediaEntryLookup.ContainsKey(EntryPoint.triacceleratorDef.PediaEntry.PersistenceId))
                __instance.pediaEntryLookup.Add(EntryPoint.triacceleratorDef.PediaEntry.PersistenceId, EntryPoint.triacceleratorDef.PediaEntry);
            if (!__instance.pediaEntryLookup.ContainsKey(EntryPoint.upcceleratorDef.PediaEntry.PersistenceId))
                __instance.pediaEntryLookup.Add(EntryPoint.upcceleratorDef.PediaEntry.PersistenceId, EntryPoint.upcceleratorDef.PediaEntry);
            if (!__instance.pediaEntryLookup.ContainsKey(EntryPoint.accelefilterDef.PediaEntry.PersistenceId))
                __instance.pediaEntryLookup.Add(EntryPoint.accelefilterDef.PediaEntry.PersistenceId, EntryPoint.accelefilterDef.PediaEntry);

            List<string> idsToRemove = __instance.gameState.Pedia.UnlockedIds.ToArray()
                .Where(pediaUnlockedId => !__instance.pediaEntryLookup.ContainsKey(pediaUnlockedId)).ToList();

            idsToRemove.ForEach(x => __instance.gameState.Pedia.UnlockedIds.Remove(x));
        }
    }
}
