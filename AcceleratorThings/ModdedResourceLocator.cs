using Il2CppInterop.Runtime.Injection;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace AcceleratorThings
{
    public class ModdedResourceLocator : UnityEngine.Object
    {
        public ModdedResourceLocator() : base(ClassInjector.DerivedConstructorPointer<ModdedResourceLocator>()) => ClassInjector.DerivedConstructorBody(this);

        public bool Locate(Il2CppSystem.Object key, Il2CppSystem.Type type, out Il2CppSystem.Collections.Generic.IList<IResourceLocation> locations)
        {
            if (!CustomAddressablesPatch.customAddressablePaths.ContainsKey(key.ToString()))
            {
                locations = new Il2CppSystem.Collections.Generic.List<IResourceLocation>().Cast<Il2CppSystem.Collections.Generic.IList<IResourceLocation>>();
                return false;
            }

            var resourceLocators = new Il2CppSystem.Collections.Generic.List<IResourceLocation>();
            resourceLocators.Add(new ResourceLocationBase(key.ToString(), key.ToString(), typeof(BundledAssetProvider).FullName, type).Cast<IResourceLocation>());
            locations = resourceLocators.Cast<Il2CppSystem.Collections.Generic.IList<IResourceLocation>>();

            return true;
        }
    }
}
