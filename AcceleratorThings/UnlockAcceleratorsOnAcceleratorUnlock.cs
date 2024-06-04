using HarmonyLib;
using Il2Cpp;

namespace AcceleratorThings
{
    //[HarmonyPatch(typeof(GadgetDirector), "AddBlueprint")]
    internal static class UnlockAcceleratorsOnAcceleratorUnlock
    {
        public static void Postfix(GadgetDirector __instance, GadgetDefinition blueprint)
        {
            if (blueprint == SRLookup.Get<GadgetDefinition>("Accelerator"))
            {
                if (!__instance.HasBlueprint(EntryPoint.vacceleratorDef))
                    __instance.AddBlueprint(EntryPoint.vacceleratorDef);
                if (!__instance.HasBlueprint(EntryPoint.triacceleratorDef))
                    __instance.AddBlueprint(EntryPoint.triacceleratorDef);
                if (!__instance.HasBlueprint(EntryPoint.upcceleratorDef))
                    __instance.AddBlueprint(EntryPoint.upcceleratorDef);
                if (!__instance.HasBlueprint(EntryPoint.accelefilterDef))
                    __instance.AddBlueprint(EntryPoint.accelefilterDef);
            }
        }
    }
}
