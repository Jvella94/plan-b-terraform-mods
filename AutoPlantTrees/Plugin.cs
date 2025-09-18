// Copyright (c) Jonathan Vella, 2025
// Licensed under the GNU GENERAL PUBLIC LICENSE, Version 3.0

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace AutoPlantTrees
{
    [BepInPlugin("averax.AutoPlantTrees", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static ManualLogSource logger;

        private static ConfigEntry<bool> modEnabled;
        private static ConfigEntry<float> percentViability;

        static readonly List<string> globalProducts =
           [
                "forest_pine",
                "forest_leavesHigh",
                "forest_tropical",
                "forest_coconut",
                "forest_cactus",
           ];

        static readonly Dictionary<string, CItem> items = [];
        static readonly List<InventoryRow> InventoryRowCache = [];
        static readonly Dictionary<CItem_ContentForest, List<int2>> viablecoords = [];
        static string currentPlanetName = "";
        static float currentTemperature = 0f;
        static readonly CLifeConditions lifeConditions = new();

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            modEnabled = Config.Bind("General", "ModEnabled", true, "Enable/Disable this mod");
            percentViability = Config.Bind("General", "percentViability", 0.8f, new ConfigDescription("The % viability for planting trees", new AcceptableValueRange<float>(0f, 1f)));
            logger = Logger;

            var h = Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SSceneHud), "OnActivate")]
        static void SSceneHud_OnActivate()
        {
            if (modEnabled.Value)
            {
                CheckforNewTileData(true);
            }
        }

        private static void CheckforNewTileData(bool fromActivate)
        {
            if (GPlanet.name != currentPlanetName || (MathF.Round(GPlanet.temperatureRise) != MathF.Round(currentTemperature)))
            {
                if (fromActivate) { logger.LogInfo($"Triggered from activate"); }
                logger.LogInfo($"Detected planet change or temperature change, refreshing viable coords");
                logger.LogInfo($"Detected planet New:{GPlanet.name} Old:{currentPlanetName}");
                logger.LogInfo($"Detected temperature {MathF.Round(GPlanet.temperatureRise)} {MathF.Round(currentTemperature)}");
                currentPlanetName = GPlanet.name;
                currentTemperature = GPlanet.temperatureRise;
                RefreshPlanetTileInfo();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SSceneHud), "OnUpdate")]
        private static bool SSceneHud_OnUpdate()
        {
            if (modEnabled.Value)
            {
                CheckforNewTileData(false);
                PlantTrees();
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CItem), nameof(CItem.Init))]
        static void CItem_Init(CItem __instance)
        {
             if(items.ContainsKey(__instance.codeName)) items[__instance.codeName] = __instance;
                else items.Add(__instance.codeName, __instance);
        }

        private static void PlantTrees()
        {
            foreach (var row in InventoryRowCache)
            {
                if (row.item.nbOwned > 0)
                {
                    if (viablecoords.ContainsKey(row.item) && viablecoords[row.item].Count > 0)
                    {
                        for (int i = 0; i < viablecoords[row.item].Count; i++)
                        {
                            var coordTocheck = viablecoords[row.item][i];
                            var liferesult = lifeConditions.Check(row.item, coordTocheck, true);
                            //Double check to make sure original measurement is still valid
                            if (liferesult == CLifeConditions.Result.OK)
                            {
                                var viability = lifeConditions.GetViability(row.item, coordTocheck);
                                if (viability > percentViability.Value)
                                {
                                    logger.LogInfo($"{DateTime.Now} Planting {row.name},{coordTocheck.x},{coordTocheck.y},{viability}");
                                    row.item.Build(coordTocheck, 0, false);
                                    if (row.item.nbOwned <= 1)
                                    {
                                        break;
                                    }
                                    viablecoords[row.item].RemoveAt(i);
                                    i--;
                                    if (viablecoords[row.item].Count == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                viablecoords[row.item].RemoveAt(i);
                                i--;
                                if (viablecoords[row.item].Count == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void RefreshPlanetTileInfo()
        {
            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loading the planet tiles!");
            logger.LogInfo($"{DateTime.Now} Building InventoryRowCache");
            InventoryRowCache.Clear();
            foreach (var codeName in globalProducts)
            {
                logger.LogInfo($"{DateTime.Now} Checking Product {codeName}");
                var row = new InventoryRow();
                if (!items.TryGetValue(codeName, out CItem item))
                {
                    continue;
                }
                row.item = item as CItem_ContentForest;
                row.codeName = codeName;
                row.name = SLoc.Get("ITEM_NAME_" + codeName);
                InventoryRowCache.Add(row);
            }
            viablecoords.Clear();
            var totalHexes = GHexes.indexesRandom.Length;
            foreach (var row in InventoryRowCache)
            {
                logger.LogInfo($"{DateTime.Now} Checking row {row.name}");
                foreach (var hex in GHexes.indexesRandom)
                {
                    var coord = SMisc.IntToCoord(hex);
                    if (lifeConditions.Check(row.item, coord, true) == CLifeConditions.Result.OK)
                    {
                        if (viablecoords.ContainsKey(row.item))
                        {
                            viablecoords[row.item].Add(coord);
                        }
                        else
                        {
                            viablecoords.Add(row.item, [coord]);
                        }
                    }
                }
            }
            logger.LogInfo($"Finished scanning planet {currentPlanetName}.");
            foreach (var row in InventoryRowCache)
            {
                if (viablecoords.TryGetValue(row.item, out List<int2> values))
                {
                    logger.LogInfo($"Found {values.Count} viable coords for {row.name}");
                }
            }
        }

        internal class InventoryRow
        {
            internal string codeName;
            internal string name;
            internal CItem_ContentForest item;
        }
    }
}