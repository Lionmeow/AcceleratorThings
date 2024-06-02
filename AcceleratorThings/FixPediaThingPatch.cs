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
            // 0.5 automatically adds any persistence IDs to the SavedGame, so no need to add those manually anymore

            List<string> idsToRemove = __instance.gameState.Pedia.UnlockedIds.ToArray()
                .Where(pediaUnlockedId => !__instance.pediaEntryLookup.ContainsKey(pediaUnlockedId)).ToList();

            idsToRemove.ForEach(x => __instance.gameState.Pedia.UnlockedIds.Remove(x));
        }
    }
}
