using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace AcceleratorThings
{
    /*[HarmonyPatch]
    public static class CustomAddressablesImplPatch
    {
        public static MethodBase TargetMethod() => typeof(AddressablesImpl).GetMethods().First(x => x.GetParameters().Length == 1 &&
            x.GetParameters()[0].ParameterType == typeof(object) && x.GetGenericArguments().Length == 1);

        [HarmonyPrefix]
        public static bool LoadAssetAsyncPrefix(AddressablesImpl __instance, object key, ref object __result)
        {
            if (key == null)
                return true;
            if (key.GetType() != typeof(string))
                return true;
            if (!CustomAddressablesPatch.customAddressablePaths.ContainsKey((string)key))
                return true;

            ResourceLocatorInfo inf = new ResourceLocatorInfo();
            inf.Locator.Locate()

            __result = __instance.ResourceManager.ProvideResource(
                new ResourceLocationBase((string)key, (string)key, typeof(AssetBundleProvider).FullName, Il2CppType.Of<UnityEngine.Object>()).Cast<IResourceLocation>());
            return false;
        }
    }*/

    [HarmonyPatch]
    public static class CustomAddressablesPatch
    {
        public static Dictionary<string, UnityEngine.Object> customAddressablePaths = new Dictionary<string, UnityEngine.Object>();

        [HarmonyPatch(typeof(ResourceManager), nameof(ResourceManager.ProvideResource), typeof(IResourceLocation), typeof(Il2CppSystem.Type), typeof(bool))]
        [HarmonyPrefix]
        public static bool ProvideResourcePrefix(ResourceManager __instance, IResourceLocation location, Il2CppSystem.Type desiredType, ref AsyncOperationHandle __result)
        {
            if (location == null)
                return true;

            if (!location.PrimaryKey.StartsWith("MODDED_AcceleratorThings"))
                return true;

            var firstOrDefault = customAddressablePaths[location.PrimaryKey];
            string systemTypeName = desiredType.FullName;
            if (!systemTypeName.StartsWith("UnityEngine"))
            {
                if (desiredType.Namespace == null)
                    systemTypeName = systemTypeName.Insert(0, "Il2Cpp.");
                else
                    systemTypeName = systemTypeName.Insert(0, "Il2Cpp");
            }

            MethodInfo method = AccessTools.Method(typeof(Il2CppObjectBase), "Cast", new Type[0], new[] { AccessTools.TypeByName(systemTypeName) });
            object result = method.Invoke(firstOrDefault, null);

            MethodInfo inf = AccessTools.Method(typeof(ResourceManager), "CreateCompletedOperationInternal");
            MethodInfo genInf = inf.MakeGenericMethod(AccessTools.TypeByName(systemTypeName));

            var completedOperationInternal = genInf.Invoke(__instance, new[] { result, true, null, false } );

            var asyncOperationHandle = new AsyncOperationHandle(((Il2CppObjectBase)AccessTools.PropertyGetter(completedOperationInternal.GetType(), "InternalOp")
                .Invoke(completedOperationInternal, null)).Cast<IAsyncOperation>(), (int)AccessTools.PropertyGetter(completedOperationInternal.GetType(), 
                "m_Version").Invoke(completedOperationInternal, null));
            __result = asyncOperationHandle;
            return false;
        }
    }
}
