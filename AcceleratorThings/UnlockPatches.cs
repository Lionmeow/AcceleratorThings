﻿using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppMonomiPark.SlimeRancher.UI.Loading;
using Il2CppMonomiPark.SlimeRancher;
using MelonLoader;
using HarmonyLib;
using UnityEngine;

namespace AcceleratorThings
{
    //[HarmonyPatch(typeof(SavedGame), "Push", new[] { typeof(GameModel) })]
    internal static class PediaSetModelPotentialUnlockPatch
    {
        public static bool shouldShowPopupVac = false;
        public static bool shouldShowPopupTri = false;
        public static bool shouldShowPopupFilter = false;
        public static bool shouldShowPopupUp = false;

        public static void Postfix()
        {
            SceneContext __instance = SceneContext.Instance;
            PediaModel model = __instance.PediaDirector._pediaModel;

            if (model == null || model.unlocked == null)
                return;

            if (__instance.GadgetDirector == null)
                return;

            if (model.unlocked.Contains(SRLookup.Get<GadgetDefinition>("Accelerator").PediaEntry))
            {
                GadgetDirector dir = __instance.GadgetDirector;
                if (!model.unlocked.Contains(EntryPoint.triacceleratorDef.PediaEntry))
                    shouldShowPopupTri = true;
                if (!model.unlocked.Contains(EntryPoint.upcceleratorDef.PediaEntry))
                    shouldShowPopupUp = true;
                if (!model.unlocked.Contains(EntryPoint.vacceleratorDef.PediaEntry))
                    shouldShowPopupVac = true;
                if (!model.unlocked.Contains(EntryPoint.accelefilterDef.PediaEntry))
                    shouldShowPopupFilter = true;
            }
        }
    }

    //[HarmonyPatch(typeof(LoadingScreenView), "Awake")]
    internal static class PopupDirectorPatch
    {
        public static void Postfix(LoadingScreenView __instance)
        {
            __instance.gameObject.AddComponent<EnqueuePopupOnLoading>();
        }
    }

    [RegisterTypeInIl2Cpp]
    internal class EnqueuePopupOnLoading : SRBehaviour
    {
        public void OnDestroy()
        {

            if (SystemContext.Instance.SceneLoader.IsCurrentSceneGroupMainMenu())
                return;
            if (SceneContext.Instance == null)
                return;

            if (PediaSetModelPotentialUnlockPatch.shouldShowPopupVac)
            {
                SceneContext.Instance.PediaDirector.Unlock(EntryPoint.triacceleratorDef._pediaLink, false);
                if (SceneContext.Instance.GadgetDirector._model.blueprints.AddIfNotPresent(EntryPoint.triacceleratorDef))
                    SceneContext.Instance.PediaDirector.ShowPopupIfUnlocked(EntryPoint.vacceleratorDef._pediaLink);
                PediaSetModelPotentialUnlockPatch.shouldShowPopupVac = false;
            }
            if (PediaSetModelPotentialUnlockPatch.shouldShowPopupTri)
            {
                SceneContext.Instance.PediaDirector.Unlock(EntryPoint.upcceleratorDef._pediaLink, false);
                if (SceneContext.Instance.GadgetDirector._model.blueprints.AddIfNotPresent(EntryPoint.upcceleratorDef))
                    SceneContext.Instance.PediaDirector.ShowPopupIfUnlocked(EntryPoint.triacceleratorDef._pediaLink);
                PediaSetModelPotentialUnlockPatch.shouldShowPopupTri = false;
            }
            if (PediaSetModelPotentialUnlockPatch.shouldShowPopupUp)
            {
                SceneContext.Instance.PediaDirector.Unlock(EntryPoint.vacceleratorDef._pediaLink, false);
                if (SceneContext.Instance.GadgetDirector._model.blueprints.AddIfNotPresent(EntryPoint.vacceleratorDef))
                    SceneContext.Instance.PediaDirector.ShowPopupIfUnlocked(EntryPoint.upcceleratorDef._pediaLink);
                PediaSetModelPotentialUnlockPatch.shouldShowPopupUp = false;
            }
            if (PediaSetModelPotentialUnlockPatch.shouldShowPopupFilter)
            {
                SceneContext.Instance.PediaDirector.Unlock(EntryPoint.accelefilterDef._pediaLink, false);
                if (SceneContext.Instance.GadgetDirector._model.blueprints.AddIfNotPresent(EntryPoint.accelefilterDef))
                    SceneContext.Instance.PediaDirector.ShowPopupIfUnlocked(EntryPoint.accelefilterDef._pediaLink);
                PediaSetModelPotentialUnlockPatch.shouldShowPopupFilter = false;
            }
        }
    }
}
