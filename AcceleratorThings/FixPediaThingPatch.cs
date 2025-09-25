using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppSystem.Linq;

namespace AcceleratorThings
{
    [HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushPedia))]
    internal static class FixPediaThingPatch // stolen from mSRML because lazy
    {
        public static void Prefix(GameModel gameModel, PediaV01 pedia)
        {
            // 0.5 automatically adds any persistence IDs to the SavedGame, so no need to add those manually anymore
            PediaEntry[] pedias = SceneContext.Instance.PediaDirector.AllEntries().ToArray();
            List<string> idsToRemove = pedia.UnlockedIds.ToArray().Where(pediaUnlockedId => !pedias.Any(x => x.PersistenceId == pediaUnlockedId)).ToList();

            idsToRemove.ForEach(x => pedia.UnlockedIds.Remove(x));
        }
    }
}
