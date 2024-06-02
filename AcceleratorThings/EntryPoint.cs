﻿using AcceleratorThings;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI.Fabricator;
using Il2CppMonomiPark.SlimeRancher.UI.Pedia;
using Il2CppMonomiPark.SlimeRancher.World;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(EntryPoint), "Accelerator Things", "1.0", "Lionmeow")]

namespace AcceleratorThings
{
    [HarmonyPatch]
    public class EntryPoint : MelonMod
    {
        public static Transform prefabParent;
        internal static AssetBundle bundle = AssetBundle.LoadFromMemory(GetAsset("accelthings"));

        public static GadgetDefinition vacceleratorDef;
        public static GadgetDefinition triacceleratorDef;
        public static GadgetDefinition upcceleratorDef;
        public static GadgetDefinition accelefilterDef;

        public static GenericVacuum vac;
        public static GameObject destroyOnVacFX;
        public static GameObject vacJointPrefab;

        public static bool gcinitialized = false;
        public static bool autosaveinitialized = false;

        public static byte[] GetAsset(string path)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{path}");
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return data;
        }

        [HarmonyPatch(typeof(LookupDirector), "Awake")]
        [HarmonyPrefix]
        public static void OnLookupDirector(LookupDirector __instance)
        {
            if (autosaveinitialized)
                return;

            autosaveinitialized = true;

            vacceleratorDef = SRLookup.GetCopy<GadgetDefinition>("Accelerator");
            vacceleratorDef.name = "Vaccelerator";
            vacceleratorDef._pediaPersistenceSuffix = "vaccelerator";
            SRLookup.Get<IdentifiableTypeGroup>("GadgetUtilitiesGroup").MemberTypes.Add(vacceleratorDef);

            triacceleratorDef = SRLookup.GetCopy<GadgetDefinition>("Accelerator");
            triacceleratorDef.name = "Triaccelerator";
            triacceleratorDef._pediaPersistenceSuffix = "triaccelerator";
            SRLookup.Get<IdentifiableTypeGroup>("GadgetUtilitiesGroup").MemberTypes.Add(triacceleratorDef);

            upcceleratorDef = SRLookup.GetCopy<GadgetDefinition>("Accelerator");
            upcceleratorDef.name = "Upccelerator";
            upcceleratorDef._pediaPersistenceSuffix = "upccelerator";
            SRLookup.Get<IdentifiableTypeGroup>("GadgetUtilitiesGroup").MemberTypes.Add(upcceleratorDef);

            accelefilterDef = SRLookup.GetCopy<GadgetDefinition>("Accelerator");
            accelefilterDef.name = "Accelefilter";
            accelefilterDef._pediaPersistenceSuffix = "accelefilter";
            accelefilterDef.Type = GadgetDefinition.Types.ITEM_DISPLAY;
            SRLookup.Get<IdentifiableTypeGroup>("GadgetUtilitiesGroup").MemberTypes.Add(accelefilterDef);

            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.name.vaccelerator", "Vaccelerator");
            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.desc.vaccelerator", "A ring that not only applies a speed boost to any object that moves through it, but also projects a vacuum stream for twice as much bending of reality.");
            vacceleratorDef.localizedName = LocalizationUtil.CreateByKey("Pedia", "m.gadget.name.vaccelerator", false);
            vacceleratorDef._localizedDescription = LocalizationUtil.CreateByKey("Pedia", "m.gadget.desc.vaccelerator", false);

            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.name.triaccelerator", "Triaccelerator");
            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.desc.triaccelerator", "Three rings that work together to boost objects evenly between two separate directions. Some say that they've seen the rings bickering, but diligent work has been done to combat any sentience.");
            triacceleratorDef.localizedName = LocalizationUtil.CreateByKey("Pedia", "m.gadget.name.triaccelerator", false);
            triacceleratorDef._localizedDescription = LocalizationUtil.CreateByKey("Pedia", "m.gadget.desc.triaccelerator", false);

            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.name.upccelerator", "Upccelerator");
            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.desc.upccelerator", "An accelerator that will raise the spirits and position of any object that passes through it. Uplifting.");
            upcceleratorDef.localizedName = LocalizationUtil.CreateByKey("Pedia", "m.gadget.name.upccelerator", false);
            upcceleratorDef._localizedDescription = LocalizationUtil.CreateByKey("Pedia", "m.gadget.desc.upccelerator", false);

            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.name.accelefilter", "Accelefilter");
            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.desc.accelefilter", "A carefully-calibrated ring that will boost desired items separately from everything else. Useful if you want to make a complex storage system, or if you're just really into carrots.");
            accelefilterDef.localizedName = LocalizationUtil.CreateByKey("Pedia", "m.gadget.name.accelefilter", false);
            accelefilterDef._localizedDescription = LocalizationUtil.CreateByKey("Pedia", "m.gadget.desc.accelefilter", false);

            PediaCategory blueprintCategory = SRLookup.Get<PediaCategory>("Blueprints");

            IdentifiablePediaEntry vacPedia = ScriptableObject.CreateInstance<IdentifiablePediaEntry>();
            vacPedia.hideFlags = HideFlags.HideAndDontSave;
            vacPedia.name = "Vaccelerator";
            vacPedia._identifiableType = vacceleratorDef;
            vacPedia._title = vacceleratorDef.localizedName;
            vacPedia._description = vacceleratorDef._localizedDescription;
            blueprintCategory._items = blueprintCategory._items.AddItem(vacPedia).ToArray();
            vacceleratorDef._pediaLink = vacPedia;

            IdentifiablePediaEntry triPedia = ScriptableObject.CreateInstance<IdentifiablePediaEntry>();
            triPedia.hideFlags = HideFlags.HideAndDontSave;
            triPedia.name = "Triaccelerator";
            triPedia._identifiableType = triacceleratorDef;
            triPedia._title = triacceleratorDef.localizedName;
            triPedia._description = triacceleratorDef._localizedDescription;
            blueprintCategory._items = blueprintCategory._items.AddItem(triPedia).ToArray();
            triacceleratorDef._pediaLink = triPedia;

            IdentifiablePediaEntry upPedia = ScriptableObject.CreateInstance<IdentifiablePediaEntry>();
            upPedia.hideFlags = HideFlags.HideAndDontSave;
            upPedia.name = "Upccelerator";
            upPedia._identifiableType = upcceleratorDef;
            upPedia._title = upcceleratorDef.localizedName;
            upPedia._description = upcceleratorDef._localizedDescription;
            blueprintCategory._items = blueprintCategory._items.AddItem(upPedia).ToArray();
            upcceleratorDef._pediaLink = upPedia;

            IdentifiablePediaEntry filterPedia = ScriptableObject.CreateInstance<IdentifiablePediaEntry>();
            filterPedia.hideFlags = HideFlags.HideAndDontSave;
            filterPedia.name = "Accelefilter";
            filterPedia._identifiableType = accelefilterDef;
            filterPedia._title = accelefilterDef.localizedName;
            filterPedia._description = accelefilterDef._localizedDescription;
            blueprintCategory._items = blueprintCategory._items.AddItem(filterPedia).ToArray();
            accelefilterDef._pediaLink = filterPedia;
        }

        [HarmonyPatch(typeof(GameContext), "Start")]
        [HarmonyPrefix]
        public static void OnGameContext(GameContext __instance)
        {
            if (gcinitialized)
                return;

            gcinitialized = true;

            UnityEngine.Object[] objs = bundle.LoadAllAssets();

            Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry> vacCosts =
                new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry>();
            vacCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("RingtailPlort") });
            vacCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("DeepBrineCraft") });
            vacCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 15, identType = SRLookup.Get<IdentifiableType>("RadiantOreCraft") });

            vacceleratorDef.CraftingCosts = new Il2CppMonomiPark.SlimeRancher.UI.PurchaseCost()
            {
                componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                newbuckCost = 100,
                identCosts = vacCosts
            };

            Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry> triCosts =
                new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry>();
            triCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("RingtailPlort") });
            triCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 3, identType = SRLookup.Get<IdentifiableType>("DeepBrineCraft") });
            triCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("LavaDustCraft") });

            triacceleratorDef.CraftingCosts = new Il2CppMonomiPark.SlimeRancher.UI.PurchaseCost()
            {
                componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                newbuckCost = 100,
                identCosts = triCosts
            };

            Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry> upCosts =
                new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry>();
            upCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("RingtailPlort") });
            upCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 2, identType = SRLookup.Get<IdentifiableType>("DeepBrineCraft") });

            upcceleratorDef.CraftingCosts = new Il2CppMonomiPark.SlimeRancher.UI.PurchaseCost()
            {
                componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                newbuckCost = 50,
                identCosts = upCosts
            };

            Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry> filterCosts =
                new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry>();
            filterCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 2, identType = SRLookup.Get<IdentifiableType>("RingtailPlort") });
            filterCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 3, identType = SRLookup.Get<IdentifiableType>("DeepBrineCraft") });
            filterCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 3, identType = SRLookup.Get<IdentifiableType>("LavaDustCraft") });
            filterCosts.Add(new Il2CppMonomiPark.SlimeRancher.UI.IdentCostEntry() { amount = 5, identType = SRLookup.Get<IdentifiableType>("RadiantOreCraft") });

            accelefilterDef.CraftingCosts = new Il2CppMonomiPark.SlimeRancher.UI.PurchaseCost()
            {
                componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                newbuckCost = 200,
                identCosts = filterCosts
            };

            vacceleratorDef.icon = objs.First(x => x.name == "iconGadgetVaccelerator" && x.GetIl2CppType() == Il2CppType.Of<Sprite>()).Cast<Sprite>();
            triacceleratorDef.icon = objs.First(x => x.name == "iconGadgetTriaccelerator" && x.GetIl2CppType() == Il2CppType.Of<Sprite>()).Cast<Sprite>();
            accelefilterDef.icon = objs.First(x => x.name == "iconGadgetAccelefilter" && x.GetIl2CppType() == Il2CppType.Of<Sprite>()).Cast<Sprite>();
            upcceleratorDef.icon = objs.First(x => x.name == "iconGadgetUpccelerator" && x.GetIl2CppType() == Il2CppType.Of<Sprite>()).Cast<Sprite>();

            GameObject vaccelerator = UnityEngine.Object.Instantiate(SRLookup.Get<GameObject>("gadgetAccelerator"), prefabParent);
            vaccelerator.transform.localPosition = Vector3.zero;
            vaccelerator.GetComponent<Gadget>().identType = vacceleratorDef;
            vacceleratorDef.prefab = vaccelerator;

            GameObject model = vaccelerator.GetComponentInChildren<MeshFilter>().gameObject;
            GameObject customModel = UnityEngine.Object.Instantiate(objs.First(x => x.name == "vaccelerator").Cast<GameObject>());

            customModel.hideFlags = HideFlags.HideAndDontSave;

            customModel.transform.SetParent(model.transform);
            customModel.transform.SetSiblingIndex(0);
            customModel.transform.localPosition = Vector3.zero;
            customModel.transform.localRotation = Quaternion.identity;

            Material m = model.GetComponent<MeshRenderer>().material;
            foreach (MeshRenderer childRenderer in customModel.GetComponentsInChildren<MeshRenderer>())
            {
                childRenderer.materials = new Material[]
                {
                    m,
                    m
                };
            }

            RotateObject rotator = customModel.transform.GetChild(0).gameObject.AddComponent<RotateObject>();
            rotator.Axis = rotator.transform.forward;
            rotator.Speed = 360;

            // I would rather attach this directly to the rotating part, but the entire game crashes when I do that
            // IL2CPP truly does steal one's lifeforce
            UnityEngine.Object.Destroy(model.transform.FindChild("model_acceleratorDecals").gameObject);

            GameObject funnel = UnityEngine.Object.Instantiate(SRLookup.Get<GameObject>("plortCollector_funnel"));
            funnel.transform.parent = customModel.transform.GetChild(0);
            funnel.transform.localPosition = Vector3.zero;
            funnel.transform.localRotation = Quaternion.Euler(90, 180, 0);

            UnityEngine.Object.Destroy(model.GetComponent<MeshFilter>());
            UnityEngine.Object.Destroy(model.GetComponent<MeshRenderer>());
            UnityEngine.Object.Destroy(model.transform.parent.GetComponentInChildren<SphereCollider>());

            GameObject vaccer = customModel.transform.GetChild(2).gameObject;
            vaccer.AddComponent<TrackCollisions>();

            vac = vaccer.AddComponent<GenericVacuum>();
            vac.vacOrigin = vaccer.transform.FindChild("vac point");

            vaccer.AddComponent<SiloActivator>();
            vaccer.layer = 18;
            for (int i = 0; i < vaccer.transform.childCount; i++)
                vaccer.transform.GetChild(i).gameObject.layer = 18;

            //vaccer.GetComponentInChildren<CapsuleCollider>().transform.parent.gameObject.AddComponent<SiloActivator>();
            //vaccer.GetComponentInChildren<CapsuleCollider>().gameObject.layer = 18;
            //vaccer.GetComponentInChildren<CapsuleCollider>().gameObject.name = "c";
            //vaccer.GetComponentInChildren<CapsuleCollider>().gameObject.AddComponent<SiloVacuumer>();
            //vaccer.GetComponentInChildren<CapsuleCollider>().gameObject.AddComponent<TrackCollisions>();

            GameObject triaccelerator = UnityEngine.Object.Instantiate(SRLookup.Get<GameObject>("gadgetAccelerator"), prefabParent);
            triaccelerator.transform.localPosition = Vector3.zero;
            triaccelerator.GetComponent<Gadget>().identType = triacceleratorDef;
            triacceleratorDef.prefab = triaccelerator;

            GameObject trimodel = triaccelerator.transform.GetChild(0).gameObject;
            GameObject triaccelTrigger = trimodel.transform.GetChild(2).gameObject;

            GameObject triaccelTriggerContainer = new GameObject("AcceleratorTriggers");
            triaccelTriggerContainer.transform.SetParent(trimodel.transform);
            triaccelTriggerContainer.transform.localPosition = triaccelTrigger.transform.localPosition;
            triaccelTriggerContainer.transform.localScale = new Vector3(1.4861f, 1.4861f, 1.4861f);

            SphereCollider triggerContainerCollider = triaccelTriggerContainer.AddComponent<SphereCollider>();
            triggerContainerCollider.isTrigger = true;
            triggerContainerCollider.center = new Vector3(0, 0, -0.5f);
            triggerContainerCollider.radius = 0.5f;
            triaccelTriggerContainer.AddComponent<DoubleAccelerator>();

            triaccelTrigger.transform.localEulerAngles += new Vector3(0, 60, 0);
            triaccelTrigger.transform.localPosition += new Vector3(0.65f, 0, 0.45f);
            UnityEngine.Object.DestroyImmediate(triaccelTrigger.GetComponent<SphereCollider>());

            GameObject triaccelTrigger2 = UnityEngine.Object.Instantiate(triaccelTrigger, trimodel.transform);
            triaccelTrigger2.transform.localEulerAngles -= new Vector3(0, 120, 0);
            triaccelTrigger2.transform.localPosition -= new Vector3(1.3f, 0, 0);

            GameObject triaccelCustomModel = UnityEngine.Object.Instantiate(objs.First(x => x.name == "triaccelerator").Cast<GameObject>());
            triaccelCustomModel.transform.SetParent(trimodel.transform);
            triaccelCustomModel.transform.localPosition = Vector3.zero;
            triaccelCustomModel.transform.localEulerAngles = Vector3.zero;
            foreach (MeshRenderer rend in triaccelCustomModel.GetComponentsInChildren<MeshRenderer>())
            {
                rend.materials = new Material[]
                {
                    m,
                    m
                };
            }

            trimodel.GetComponent<MeshCollider>().sharedMesh = objs.First(x => x.name == "triaccelerator_COL").Cast<GameObject>().GetComponent<MeshFilter>().mesh;
            UnityEngine.Object.Destroy(trimodel.GetComponent<MeshFilter>());
            UnityEngine.Object.Destroy(trimodel.GetComponent<MeshRenderer>());
            UnityEngine.Object.Destroy(trimodel.transform.FindChild("model_acceleratorDecals").gameObject);

            GameObject upccelerator = UnityEngine.Object.Instantiate(SRLookup.Get<GameObject>("gadgetAccelerator"), prefabParent);
            upccelerator.transform.localPosition = Vector3.zero;
            upccelerator.GetComponent<Gadget>().identType = upcceleratorDef;
            upcceleratorDef.prefab = upccelerator;

            GameObject upmodel = upccelerator.transform.GetChild(0).gameObject;

            GameObject upaccelCustomModel = UnityEngine.Object.Instantiate(objs.First(x => x.name == "upccelerator").Cast<GameObject>());
            upaccelCustomModel.transform.SetParent(upmodel.transform);
            upaccelCustomModel.transform.localPosition = Vector3.zero;
            upaccelCustomModel.transform.localEulerAngles = Vector3.zero;
            foreach (MeshRenderer rend in upaccelCustomModel.GetComponentsInChildren<MeshRenderer>())
            {
                rend.materials = new Material[]
                {
                    m,
                    m
                };
            }

            Transform upccelTrigger = upmodel.transform.GetChild(2);
            upccelTrigger.localEulerAngles += new Vector3(-30, 0, 0);
            upccelTrigger.localPosition += new Vector3(0, 0.102f, -0.659f);

            Transform upfx = upccelerator.transform.GetChild(1);
            upfx.localEulerAngles += new Vector3(30, 0, 0);
            upfx.localPosition += new Vector3(0, -0.102f, 0.659f);

            upmodel.GetComponent<MeshCollider>().sharedMesh = objs.First(x => x.name == "upccelerator_COL").Cast<GameObject>().GetComponent<MeshFilter>().mesh;
            UnityEngine.Object.Destroy(upmodel.GetComponent<MeshFilter>());
            UnityEngine.Object.Destroy(upmodel.GetComponent<MeshRenderer>());
            UnityEngine.Object.Destroy(upmodel.transform.FindChild("model_acceleratorDecals").gameObject);

            GameObject accelefilter = UnityEngine.Object.Instantiate(SRLookup.Get<GameObject>("gadgetAccelerator"), prefabParent);
            accelefilter.transform.localPosition = Vector3.zero;
            accelefilter.GetComponent<Gadget>().identType = accelefilterDef;
            accelefilterDef.prefab = accelefilter;

            GameObject filtermodel = accelefilter.transform.GetChild(0).gameObject;
            GameObject filterTrigger = filtermodel.transform.GetChild(2).gameObject;

            GameObject filterTriggerContainer = new GameObject("AcceleratorTriggers");
            filterTriggerContainer.transform.SetParent(filtermodel.transform);
            filterTriggerContainer.transform.localPosition = filterTrigger.transform.localPosition;
            filterTriggerContainer.transform.localScale = new Vector3(1.4861f, 1.4861f, 1.4861f);

            SphereCollider filterContainerCollider = filterTriggerContainer.AddComponent<SphereCollider>();
            filterContainerCollider.isTrigger = true;
            filterContainerCollider.center = new Vector3(0, 0, -0.413f);
            filterContainerCollider.radius = 0.5f;
            filterTriggerContainer.AddComponent<FilterAccelerator>();

            filterTrigger.transform.localPosition += new Vector3(0, 0, 0.467f);
            UnityEngine.Object.DestroyImmediate(filterTrigger.GetComponent<SphereCollider>());

            GameObject filterTrigger2 = UnityEngine.Object.Instantiate(filterTrigger, filtermodel.transform);
            filterTrigger2.transform.localEulerAngles -= new Vector3(0, 90, 0);
            filterTrigger2.transform.localPosition -= new Vector3(1.098f, 0, 0.485f);

            GameObject filterCustomModel = UnityEngine.Object.Instantiate(objs.First(x => x.name == "accelefilter").Cast<GameObject>());
            filterCustomModel.transform.SetParent(filtermodel.transform);
            filterCustomModel.transform.localPosition = Vector3.zero;
            filterCustomModel.transform.localEulerAngles = Vector3.zero;
            foreach (MeshRenderer rend in filterCustomModel.GetComponentsInChildren<MeshRenderer>())
            {
                rend.materials = new Material[]
                {
                    m,
                    m
                };
            }

            GameObject decorItemDisplay = UnityEngine.Object.Instantiate(SRLookup.Get<GameObject>("decorItemDisplay"), filtermodel.transform);
            UnityEngine.Object.Destroy(decorItemDisplay.GetComponent<Gadget>());
            UnityEngine.Object.Destroy(decorItemDisplay.GetComponent<RegionMember>());
            decorItemDisplay.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            decorItemDisplay.transform.localPosition = new Vector3(0, 1.8f, 0);
            Transform decorItemDisplayModel = decorItemDisplay.transform.FindChild("model_techItemDisplay");
            UnityEngine.Object.Destroy(decorItemDisplayModel.transform.FindChild("techItemDisplay_decals").gameObject);
            UnityEngine.Object.Destroy(decorItemDisplay.transform.FindChild("techDepositorPort_inputOutput").gameObject);
            decorItemDisplayModel.FindChild("techItemDisplay_base").GetComponent<MeshFilter>().mesh =
                objs.First(x => x.name == "accelefilter_itembase").Cast<GameObject>().GetComponent<MeshFilter>().mesh;
            decorItemDisplayModel.FindChild("ItemAttachPoint/ItemAttachPoint_offset").localScale = new Vector3(0.933f, 0.933f, 0.933f);
            decorItemDisplay.transform.FindChild("triggerDeposit").localPosition = new Vector3(0, 2.2f, 0);
            decorItemDisplay.transform.FindChild("triggerDeposit").localEulerAngles = new Vector3(-90, 0, 0);

            filtermodel.GetComponent<MeshCollider>().sharedMesh = objs.First(x => x.name == "accelefilter_COL").Cast<GameObject>().GetComponent<MeshFilter>().mesh;
            UnityEngine.Object.Destroy(filtermodel.GetComponent<MeshFilter>());
            UnityEngine.Object.Destroy(filtermodel.GetComponent<MeshRenderer>());
            UnityEngine.Object.Destroy(decorItemDisplay.GetComponentInChildren<CapsuleCollider>());
            UnityEngine.Object.Destroy(filtermodel.transform.FindChild("model_acceleratorDecals").gameObject);
        }

        [HarmonyPatch(typeof(VacuumItem), "Awake")]
        [HarmonyPrefix]
        public static void VacuumItemAwakePrefix(VacuumItem __instance)
        {
            destroyOnVacFX = __instance.DestroyOnVacFX;
            vacJointPrefab = __instance.VacJointPrefab;
        }

        public static GenericVacuum currVacuum;

        [HarmonyPatch(typeof(SiloCatcher), "OnTriggerStay")]
        [HarmonyPrefix]
        public static void GetCurrVacuumPatch(SiloCatcher __instance, Collider collider) => currVacuum = collider.GetComponentInParent<GenericVacuum>();

        [HarmonyPatch(typeof(SiloCatcher), "OnTriggerStay")]
        [HarmonyFinalizer]
        public static void ClearCurrVacuumPatch() => currVacuum = null;

        [HarmonyPatch(typeof(VacuumItem), "ForceJoint")]
        [HarmonyPrefix]
        public static void ForceJointToCurrVacuumPatch(Vacuumable vacuumable)
        {
            if (currVacuum)
                currVacuum.ForceJoint(vacuumable);
        }
    }
}
