using AcceleratorThings;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Generator.Extensions;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Event;
using Il2CppMonomiPark.SlimeRancher.Event.Query;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.Shop;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.Util;
using Il2CppMonomiPark.SlimeRancher.World;
using Il2CppSystem.Linq;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

[assembly: MelonInfo(typeof(EntryPoint), "Accelerator Things", "1.4", "Lionmeow")]

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
        public static GadgetDefinition bigcceleratorDef;

        public static ShopCategorySourceRuleSet acceleratorsRuleset;
        public static ShopFixedItemsTable acceleratorsTable;
        public static ShopCategorySourceLink acceleratorsLink;

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

            bigcceleratorDef = SRLookup.GetCopy<GadgetDefinition>("Accelerator");
            bigcceleratorDef.name = "Bigccelerator";
            bigcceleratorDef._pediaPersistenceSuffix = "bigccelerator";
            SRLookup.Get<IdentifiableTypeGroup>("GadgetUtilitiesGroup").MemberTypes.Add(bigcceleratorDef);

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

            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.name.bigccelerator", "Bigccelerator");
            LocalizationUtil.GetTable("Pedia").AddEntry("m.gadget.desc.bigccelerator", "This oversized ring will give ANYTHING a boost of high speed. Even yourself. Side effects may include: getting launched into space.");
            bigcceleratorDef.localizedName = LocalizationUtil.CreateByKey("Pedia", "m.gadget.name.bigccelerator", false);
            bigcceleratorDef._localizedDescription = LocalizationUtil.CreateByKey("Pedia", "m.gadget.desc.bigccelerator", false);

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

            IdentifiablePediaEntry bigPedia = ScriptableObject.CreateInstance<IdentifiablePediaEntry>();
            bigPedia.hideFlags = HideFlags.HideAndDontSave;
            bigPedia.name = "Bigccelerator";
            bigPedia._identifiableType = bigcceleratorDef;
            bigPedia._title = bigcceleratorDef.localizedName;
            bigPedia._description = bigcceleratorDef._localizedDescription;
            blueprintCategory._items = blueprintCategory._items.AddItem(bigPedia).ToArray();
            bigcceleratorDef._pediaLink = bigPedia;
        }

        [HarmonyPatch(typeof(GameContext), "Start")]
        [HarmonyPostfix]
        public static void OnGameContext(GameContext __instance)
        {
            if (gcinitialized)
                return;

            gcinitialized = true;

            UnityEngine.Object[] objs = bundle.LoadAllAssets();

            CustomAddressablesPatch.customAddressablePaths["MODDED_AcceleratorThings/Vaccelerator"] = vacceleratorDef;
            CustomAddressablesPatch.customAddressablePaths["MODDED_AcceleratorThings/Triaccelerator"] = triacceleratorDef;
            CustomAddressablesPatch.customAddressablePaths["MODDED_AcceleratorThings/Upccelerator"] = upcceleratorDef;
            CustomAddressablesPatch.customAddressablePaths["MODDED_AcceleratorThings/Accelefilter"] = accelefilterDef;
            CustomAddressablesPatch.customAddressablePaths["MODDED_AcceleratorThings/Bigccelerator"] = bigcceleratorDef;

            ClassInjector.RegisterTypeInIl2Cpp<ModdedResourceLocator>(new RegisterTypeOptions()
            {
                Interfaces = new Il2CppInterfaceCollection(new[] { typeof(IResourceLocator) })
            });
            Addressables.AddResourceLocator(new ModdedResourceLocator().Cast<IResourceLocator>());

            // add blueprints to shop

            acceleratorsRuleset = ScriptableObject.CreateInstance<ShopCategorySourceRuleSet>();
            acceleratorsRuleset.hideFlags = HideFlags.HideAndDontSave;
            acceleratorsRuleset.name = "RangeExchange_AcceleratorThingsRuleSet";

            Il2CppSystem.Collections.Generic.List<IGameQueryComponent> queryComponents = new Il2CppSystem.Collections.Generic.List<IGameQueryComponent>();
            queryComponents.Add(GadgetEventQueryComponent.CreateQueryComponent(new AssetReferenceT<GadgetEventProducer>("fb4d5e5097d315d4cbc6e8f6d7fd7a3a") // GadgetBluePrintObtained
                .LoadAsset<GadgetEventProducer>().WaitForCompletion(), SRLookup.Get<GadgetDefinition>("Accelerator"), 1).Cast<IGameQueryComponent>());
            acceleratorsRuleset._allItemsAvailableCondition = CompositeQueryComponent.CreateCompositeQueryComponent(CompositeQueryComponent.BoolOperation.ALL_OF,
            queryComponents.Cast<Il2CppSystem.Collections.Generic.IEnumerable<IGameQueryComponent>>());

            acceleratorsRuleset._assetGuid = "MODDED_AcceleratorThings/RuleSet";
            acceleratorsRuleset._categoryLink = new AssetReferenceT<ShopItemCategoryDescription>("b99fdf9ac7d2f2d4c842bd747045ea73"); // RangeExchange (fixed shop category)
            acceleratorsRuleset._countLimit = new Il2CppMonomiPark.SlimeRancher.Util.Optional<int>();
            acceleratorsRuleset._randomItemRestockFrequency = ShopRandomRestockFrequency.NEVER_RESET;
            acceleratorsRuleset._sortIndex = 4;

            CustomAddressablesPatch.customAddressablePaths["MODDED_AcceleratorThings/RuleSet"] = acceleratorsRuleset;

            acceleratorsTable = ScriptableObject.CreateInstance<ShopFixedItemsTable>();
            acceleratorsTable.hideFlags = HideFlags.HideAndDontSave;
            acceleratorsTable.name = "RangeExchange_AcceleratorThingsItemsTable";
            acceleratorsTable._itemListings = new ShopFixedItemsTable.ItemEntry[]
            {
                new ShopFixedItemsTable.ItemEntry()
                {
                    ItemCost = new ShopItemCost()
                    {
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 750
                        },
                        UnlockType = ShopItemUnlockType.ITEM_BLUEPRINT,
                        ShopItem = new ShopItemAssetReference()
                        {
                            _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Vaccelerator"),
                            _filterFlags = ShopItemFilterFlags.GADGET,
                            _identifiableReferenceId = vacceleratorDef.ReferenceId
                        }
                    },
                    AcquireLimit = new Il2CppMonomiPark.SlimeRancher.Util.Optional<int>()
                },
                new ShopFixedItemsTable.ItemEntry()
                {
                    ItemCost = new ShopItemCost()
                    {
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 500
                        },
                        UnlockType = ShopItemUnlockType.ITEM_BLUEPRINT,
                        ShopItem = new ShopItemAssetReference()
                        {
                            _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Triaccelerator"),
                            _filterFlags = ShopItemFilterFlags.GADGET,
                            _identifiableReferenceId = triacceleratorDef.ReferenceId
                        }
                    },
                    AcquireLimit = new Il2CppMonomiPark.SlimeRancher.Util.Optional<int>()
                },
                new ShopFixedItemsTable.ItemEntry()
                {
                    ItemCost = new ShopItemCost()
                    {
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 250
                        },
                        UnlockType = ShopItemUnlockType.ITEM_BLUEPRINT,
                        ShopItem = new ShopItemAssetReference()
                        {
                            _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Upccelerator"),
                            _filterFlags = ShopItemFilterFlags.GADGET,
                            _identifiableReferenceId = upcceleratorDef.ReferenceId
                        }
                    },
                    AcquireLimit = new Il2CppMonomiPark.SlimeRancher.Util.Optional<int>()
                },
                new ShopFixedItemsTable.ItemEntry()
                {
                    ItemCost = new ShopItemCost()
                    {
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 1000
                        },
                        UnlockType = ShopItemUnlockType.ITEM_BLUEPRINT,
                        ShopItem = new ShopItemAssetReference()
                        {
                            _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Accelefilter"),
                            _filterFlags = ShopItemFilterFlags.GADGET,
                            _identifiableReferenceId = accelefilterDef.ReferenceId
                        }
                    },
                    AcquireLimit = new Il2CppMonomiPark.SlimeRancher.Util.Optional<int>()
                },
                new ShopFixedItemsTable.ItemEntry()
                {
                    ItemCost = new ShopItemCost()
                    {
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 500
                        },
                        UnlockType = ShopItemUnlockType.ITEM_BLUEPRINT,
                        ShopItem = new ShopItemAssetReference()
                        {
                            _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Bigccelerator"),
                            _filterFlags = ShopItemFilterFlags.GADGET,
                            _identifiableReferenceId = bigcceleratorDef.ReferenceId
                        }
                    },
                    AcquireLimit = new Il2CppMonomiPark.SlimeRancher.Util.Optional<int>()
                }
            };
            acceleratorsTable.OnAfterDeserialize(); // puts item entries into the used lists

            acceleratorsLink = ScriptableObject.CreateInstance<ShopCategorySourceLink>();
            acceleratorsLink.hideFlags = HideFlags.HideAndDontSave;
            acceleratorsLink.name = "RangeExchange_AcceleratorThingsSourceLink";
            acceleratorsLink._linkedSource = acceleratorsTable;
            acceleratorsLink._owningRuleSet = new AssetReferenceT<ShopCategorySourceRuleSet>("MODDED_AcceleratorThings/RuleSet");
            acceleratorsLink.OwningRuleSet.LoadAsset(); // likely not needed, but it could help
            acceleratorsLink._sortIndex = 0;

            CustomAddressablesPatch.customAddressablePaths["MODDED_AcceleratorThings/SourceLink"] = acceleratorsLink;

            // add to random shop

            CompositeQueryComponent emptyQuery = CompositeQueryComponent.CreateCompositeQueryComponent(CompositeQueryComponent.BoolOperation.ALL_OF, 
                new Il2CppSystem.Collections.Generic.List<IGameQueryComponent>().Cast<Il2CppSystem.Collections.Generic.IEnumerable<IGameQueryComponent>>());

            ShopRandomItemsTable t3RandomTable = new AssetReferenceT<ShopRandomItemsTable>("6fa42cb350c10064eaaf82bb9d9d7bf0").LoadAsset().WaitForCompletion();
            t3RandomTable.Items = t3RandomTable.Items.AddItem(new ShopRandomItemsTable.ItemRateEntry()
            {
                AvailableCondition = emptyQuery,
                OccurenceWeight = 1,
                UnlockType = ShopItemUnlockType.INDIVIDUAL_ITEM,
                Item = new ShopItemAssetReference()
                {
                    _filterFlags = ShopItemFilterFlags.GADGET,
                    _identifiableReferenceId = "GadgetDefinition.Upccelerator",
                    _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Upccelerator")
                },
                Costs = new ShopRandomItemsTable.CostRateEntry[]
                {
                    new ShopRandomItemsTable.CostRateEntry()
                    {
                        AcquireLimit = new Optional<int>().WithValue(1),
                        OccurenceWeight = 0,
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 50
                        }
                    }
                }
            }).ToArray();

            ShopRandomItemsTable t4RandomTable = new AssetReferenceT<ShopRandomItemsTable>("4bc09a5512ed2c346ba6c23c60ecd027").LoadAsset().WaitForCompletion();
            t4RandomTable.Items = t4RandomTable.Items.AddItem(new ShopRandomItemsTable.ItemRateEntry()
            {
                AvailableCondition = emptyQuery,
                OccurenceWeight = 1,
                UnlockType = ShopItemUnlockType.INDIVIDUAL_ITEM,
                Item = new ShopItemAssetReference()
                {
                    _filterFlags = ShopItemFilterFlags.GADGET,
                    _identifiableReferenceId = "GadgetDefinition.Triccelerator",
                    _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Triccelerator")
                },
                Costs = new ShopRandomItemsTable.CostRateEntry[]
                {
                    new ShopRandomItemsTable.CostRateEntry()
                    {
                        AcquireLimit = new Optional<int>().WithValue(1),
                        OccurenceWeight = 0,
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 100
                        }
                    }
                }
            }).AddItem(new ShopRandomItemsTable.ItemRateEntry()
            {
                AvailableCondition = emptyQuery,
                OccurenceWeight = 1,
                UnlockType = ShopItemUnlockType.INDIVIDUAL_ITEM,
                Item = new ShopItemAssetReference()
                {
                    _filterFlags = ShopItemFilterFlags.GADGET,
                    _identifiableReferenceId = "GadgetDefinition.Bigccelerator",
                    _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Bigccelerator")
                },
                Costs = new ShopRandomItemsTable.CostRateEntry[]
                {
                    new ShopRandomItemsTable.CostRateEntry()
                    {
                        AcquireLimit = new Optional<int>().WithValue(1),
                        OccurenceWeight = 0,
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 100
                        }
                    }
                }
            }).ToArray();

            ShopRandomItemsTable t5RandomTable = new AssetReferenceT<ShopRandomItemsTable>("2ed966ba45a5dc84bb8f70eab16d1c89").LoadAsset().WaitForCompletion();
            t5RandomTable.Items = t5RandomTable.Items.AddItem(new ShopRandomItemsTable.ItemRateEntry()
            {
                AvailableCondition = emptyQuery,
                OccurenceWeight = 1,
                UnlockType = ShopItemUnlockType.INDIVIDUAL_ITEM,
                Item = new ShopItemAssetReference()
                {
                    _filterFlags = ShopItemFilterFlags.GADGET,
                    _identifiableReferenceId = "GadgetDefinition.Vaccelerator",
                    _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Vaccelerator")
                },
                Costs = new ShopRandomItemsTable.CostRateEntry[]
                {
                    new ShopRandomItemsTable.CostRateEntry()
                    {
                        AcquireLimit = new Optional<int>().WithValue(1),
                        OccurenceWeight = 0,
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 200
                        }
                    }
                }
            }).AddItem(new ShopRandomItemsTable.ItemRateEntry()
            {
                AvailableCondition = emptyQuery,
                OccurenceWeight = 1,
                UnlockType = ShopItemUnlockType.INDIVIDUAL_ITEM,
                Item = new ShopItemAssetReference()
                {
                    _filterFlags = ShopItemFilterFlags.GADGET,
                    _identifiableReferenceId = "GadgetDefinition.Accelefilter",
                    _assetReference = new AssetReferenceT<IdentifiableType>("MODDED_AcceleratorThings/Accelefilter")
                },
                Costs = new ShopRandomItemsTable.CostRateEntry[]
                {
                    new ShopRandomItemsTable.CostRateEntry()
                    {
                        AcquireLimit = new Optional<int>().WithValue(1),
                        OccurenceWeight = 0,
                        PurchaseCost = new PurchaseCost()
                        {
                            componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                            identCosts = new Il2CppSystem.Collections.Generic.List<IdentCostEntry>(),
                            newbuckCost = 250
                        }
                    }
                }
            }).ToArray();

            // more registry things that couldn't've been done pre-LookupDirector

            Il2CppSystem.Collections.Generic.List<IdentCostEntry> vacCosts =
                new Il2CppSystem.Collections.Generic.List<IdentCostEntry>();
            vacCosts.Add(new IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("RingtailPlort") });
            vacCosts.Add(new IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("DeepBrineCraft") });
            vacCosts.Add(new IdentCostEntry() { amount = 15, identType = SRLookup.Get<IdentifiableType>("RadiantOreCraft") });

            vacceleratorDef.CraftingCosts = new PurchaseCost()
            {
                componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                newbuckCost = 100,
                identCosts = vacCosts
            };

            Il2CppSystem.Collections.Generic.List<IdentCostEntry> triCosts =
                new Il2CppSystem.Collections.Generic.List<IdentCostEntry>();
            triCosts.Add(new IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("RingtailPlort") });
            triCosts.Add(new IdentCostEntry() { amount = 3, identType = SRLookup.Get<IdentifiableType>("DeepBrineCraft") });
            triCosts.Add(new IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("LavaDustCraft") });

            triacceleratorDef.CraftingCosts = new PurchaseCost()
            {
                componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                newbuckCost = 100,
                identCosts = triCosts
            };

            Il2CppSystem.Collections.Generic.List<IdentCostEntry> upCosts =
                new Il2CppSystem.Collections.Generic.List<IdentCostEntry>();
            upCosts.Add(new IdentCostEntry() { amount = 1, identType = SRLookup.Get<IdentifiableType>("RingtailPlort") });
            upCosts.Add(new IdentCostEntry() { amount = 2, identType = SRLookup.Get<IdentifiableType>("DeepBrineCraft") });

            upcceleratorDef.CraftingCosts = new PurchaseCost()
            {
                componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                newbuckCost = 50,
                identCosts = upCosts
            };

            Il2CppSystem.Collections.Generic.List<IdentCostEntry> filterCosts =
                new Il2CppSystem.Collections.Generic.List<IdentCostEntry>();
            filterCosts.Add(new IdentCostEntry() { amount = 2, identType = SRLookup.Get<IdentifiableType>("RingtailPlort") });
            filterCosts.Add(new IdentCostEntry() { amount = 3, identType = SRLookup.Get<IdentifiableType>("DeepBrineCraft") });
            filterCosts.Add(new IdentCostEntry() { amount = 3, identType = SRLookup.Get<IdentifiableType>("LavaDustCraft") });
            filterCosts.Add(new IdentCostEntry() { amount = 5, identType = SRLookup.Get<IdentifiableType>("RadiantOreCraft") });

            accelefilterDef.CraftingCosts = new PurchaseCost()
            {
                componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                newbuckCost = 200,
                identCosts = filterCosts
            };

            Il2CppSystem.Collections.Generic.List<IdentCostEntry> bigCosts =
                new Il2CppSystem.Collections.Generic.List<IdentCostEntry>();
            bigCosts.Add(new IdentCostEntry() { amount = 2, identType = SRLookup.Get<IdentifiableType>("RingtailPlort") });
            bigCosts.Add(new IdentCostEntry() { amount = 4, identType = SRLookup.Get<IdentifiableType>("DeepBrineCraft") });

            bigcceleratorDef.CraftingCosts = new PurchaseCost()
            {
                componentCosts = new Il2CppSystem.Collections.Generic.List<Il2CppMonomiPark.SlimeRancher.Player.Component.UpgradeComponent>(),
                newbuckCost = 100,
                identCosts = bigCosts
            };

            vacceleratorDef.icon = objs.First(x => x.name == "iconGadgetVaccelerator" && x.GetIl2CppType() == Il2CppType.Of<Sprite>()).Cast<Sprite>();
            triacceleratorDef.icon = objs.First(x => x.name == "iconGadgetTriaccelerator" && x.GetIl2CppType() == Il2CppType.Of<Sprite>()).Cast<Sprite>();
            accelefilterDef.icon = objs.First(x => x.name == "iconGadgetAccelefilter" && x.GetIl2CppType() == Il2CppType.Of<Sprite>()).Cast<Sprite>();
            upcceleratorDef.icon = objs.First(x => x.name == "iconGadgetUpccelerator" && x.GetIl2CppType() == Il2CppType.Of<Sprite>()).Cast<Sprite>();
            bigcceleratorDef.icon = objs.First(x => x.name == "iconGadgetBigccelerator" && x.GetIl2CppType() == Il2CppType.Of<Sprite>()).Cast<Sprite>();

            // vaccelerator

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
            vaccer.layer = 18; // VacCone (layer that allows interaction with silo activators)
            for (int i = 0; i < vaccer.transform.childCount; i++)
                vaccer.transform.GetChild(i).gameObject.layer = 18;

            // triaccelerator

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

            // upccelerator

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

            // accelefilter

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

            // bigccelerator

            GameObject bigccelerator = UnityEngine.Object.Instantiate(SRLookup.Get<GameObject>("gadgetAccelerator"), prefabParent);
            bigccelerator.transform.localPosition = Vector3.zero;
            bigccelerator.GetComponent<Gadget>().identType = bigcceleratorDef;
            bigcceleratorDef.prefab = bigccelerator;

            GameObject bigmodel = bigccelerator.transform.GetChild(0).gameObject;

            GameObject bigccelCustomModel = UnityEngine.Object.Instantiate(objs.First(x => x.name == "bigccelerator").Cast<GameObject>());
            bigccelCustomModel.transform.SetParent(bigmodel.transform);
            bigccelCustomModel.transform.localPosition = Vector3.zero;
            bigccelCustomModel.transform.localEulerAngles = Vector3.zero;
            foreach (MeshRenderer rend in bigccelCustomModel.GetComponentsInChildren<MeshRenderer>())
            {
                rend.materials = new Material[]
                {
                    m,
                    m
                };
            }

            Transform bigccelTrigger = bigmodel.transform.GetChild(2);
            bigccelTrigger.localPosition += new Vector3(0, 0.5f, 0);
            bigccelTrigger.localScale = new Vector3(2, 2, 2);
            Accelerator bigccelComponent = bigccelTrigger.GetComponent<Accelerator>();
            bigccelComponent._acceleratableTypeGroup = SRLookup.Get<IdentifiableTypeGroup>("IdentifiableTypesGroup");
            bigccelComponent._accelerationForce = new Vector3(0, 0, 50);
            PlayerAccelerator bigccelPlayer = bigccelTrigger.gameObject.AddComponent<PlayerAccelerator>();
            PlayerAccelerator.launchedCue = bigccelComponent._itemAcceleratedCue;

            Transform bigfx = bigccelerator.transform.GetChild(1);
            bigfx.localPosition += new Vector3(0, 0.5f, 0);
            bigfx.localScale = new Vector3(2, 2, 2);

            bigmodel.GetComponent<MeshCollider>().sharedMesh = objs.First(x => x.name == "bigccelerator_COL").Cast<GameObject>().GetComponentInChildren<MeshFilter>().mesh;
            UnityEngine.Object.Destroy(bigmodel.GetComponent<MeshFilter>());
            UnityEngine.Object.Destroy(bigmodel.GetComponent<MeshRenderer>());
            UnityEngine.Object.Destroy(bigmodel.transform.FindChild("model_acceleratorDecals").gameObject);
        }

        [HarmonyPatch(typeof(AddressablesImpl), "LoadResourceLocationsAsync", new [] { typeof(Il2CppSystem.Object), typeof(Il2CppSystem.Type) })]
        [HarmonyPostfix]
        public static void LoadResourceLocations(Il2CppSystem.Object key, Il2CppSystem.Type type, 
            ref AsyncOperationHandle<Il2CppSystem.Collections.Generic.IList<IResourceLocation>> __result)
        {
            // adds the modded category to the list of categories to be loaded
            if (key != null && key.ToString().Contains("Group_ShopCategory"))
            {
                Il2CppSystem.Collections.Generic.List<IResourceLocation> locations =
                    new Il2CppSystem.Collections.Generic.List<IResourceLocation>(__result.Result.Cast<Il2CppSystem.Collections.Generic.IEnumerable<IResourceLocation>>());

                locations.Add(new ResourceLocationBase("MODDED_AcceleratorThings/RuleSet", "MODDED_AcceleratorThings/RuleSet",
                    typeof(BundledAssetProvider).FullName, Il2CppType.Of<ShopCategorySourceRuleSet>()).Cast<IResourceLocation>());
                locations.Add(new ResourceLocationBase("MODDED_AcceleratorThings/SourceLink", "MODDED_AcceleratorThings/SourceLink",
                    typeof(BundledAssetProvider).FullName, Il2CppType.Of<ShopCategorySourceLink>()).Cast<IResourceLocation>());

                __result = Addressables.ResourceManager.CreateCompletedOperationInternal(locations.Cast<Il2CppSystem.Collections.Generic.IList<IResourceLocation>>(),
                    true, null, false);
            }
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
