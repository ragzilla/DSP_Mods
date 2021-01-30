using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using BepInEx;

namespace DSP_RecipeDumper
{
    [BepInPlugin("us.evilgeni.dsp_recipedumper", "Recipe Dumper Plug-In", "1.0.0.0")]
    [BepInProcess("DSPGAME.exe")]
    public class DSP_RecipeDumperPlugin : BaseUnityPlugin
    {
        internal void Awake()
        {
            var harmony = new Harmony("us.evilgeni.dsp_recipedumper"); 
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(GlobalObject), "LoadVersions")]
        public class GlobalObject_LoadVersions
        {
            public static void Postfix(GlobalObject __instance)
            {
                DSP_RecipeDumper.Logger.Log("[meta]");
                DSP_RecipeDumper.Logger.Log("version = \'" + GameConfig.gameVersion.ToFullString() + "\'");
                DSP_RecipeDumper.Logger.Log("build = " + GameConfig.build);
            }
        }

        [HarmonyPatch(typeof(GameMain), "Begin")]
        public class GameMain_Begin
        {
            public static void Postfix(GlobalObject __instance)
            {
                DSP_RecipeDumper.Logger.Close();
            }
        }

        [HarmonyPatch(typeof(RecipeProto), "Preload")]
        public class RecipeProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(RecipeProto __instance)
            {
                var name = DSP_RecipeDumper.Translate.Get("recipe" + __instance.ID);
                DSP_RecipeDumper.Logger.Log("[recipe." + name + "]");
                DSP_RecipeDumper.Logger.Log("name = \'" + __instance.name + "\'");
                DSP_RecipeDumper.Logger.Log("timeSpend = " + __instance.TimeSpend);
                if (__instance.Type > 0) 
                    DSP_RecipeDumper.Logger.Log("madeFrom = \'" + __instance.madeFromString + "\'");
                DSP_RecipeDumper.Logger.Log("type = \'" + __instance.Type + "\'");
                DSP_RecipeDumper.Logger.Log("handcraft = " + __instance.Handcraft.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("explicit = " + __instance.Explicit.ToString().ToLower());
                if (__instance.preTech != (TechProto)null)
                    DSP_RecipeDumper.Logger.Log("preTech = \'" + __instance.preTech.Name.Translate() + "\'");
                if (__instance.IconPath.Length > 0)
                    DSP_RecipeDumper.Logger.Log("iconPath = \'" + __instance.IconPath + "\'");
                if (__instance.Items.Length > 0)
                {
                    DSP_RecipeDumper.Logger.Log("[recipe." + name + ".items]");
                    for (int i = 0; i < __instance.Items.Length; i++)
                        DSP_RecipeDumper.Logger.Log(DSP_RecipeDumper.Translate.Get("item" + __instance.Items[i]) + " = " + __instance.ItemCounts[i]);
                }
                if (__instance.Results.Length > 0)
                {
                    DSP_RecipeDumper.Logger.Log("[recipe." + name + ".results]");
                    for (int i = 0; i < __instance.Results.Length; i++)
                        DSP_RecipeDumper.Logger.Log(DSP_RecipeDumper.Translate.Get("item" + __instance.Results[i]) + " = " + __instance.ResultCounts[i]);
                }
            }
        }

        [HarmonyPatch(typeof(ItemProto), "Preload")]
        public class ItemProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(ItemProto __instance)
            {
                var name = DSP_RecipeDumper.Translate.Get("item" + __instance.ID);
                DSP_RecipeDumper.Logger.Log("[item." + name + "]");
                DSP_RecipeDumper.Logger.Log("name = \'" + __instance.name + "\'");
                DSP_RecipeDumper.Logger.Log("stackSize = " + __instance.StackSize);
                DSP_RecipeDumper.Logger.Log("type = \'" + __instance.typeString + "\'");
                if (__instance.FuelType > 0)
                    DSP_RecipeDumper.Logger.Log("fuelType = \'" + __instance.fuelTypeString + "\'");
                if (__instance.HeatValue > 0)
                    DSP_RecipeDumper.Logger.Log("heatValue = " + __instance.HeatValue);
                if (__instance.miningFrom.Length > 0)
                    DSP_RecipeDumper.Logger.Log("miningFrom = \'" + __instance.miningFrom.Trim() + "\'");
                if (__instance.produceFrom.Length > 0)
                    DSP_RecipeDumper.Logger.Log("produceFrom = \'" + __instance.produceFrom.Trim() + "\'");
                DSP_RecipeDumper.Logger.Log("description = '''" + __instance.description.Trim() + "'''");
                if (__instance.preTech != (TechProto)null)
                    DSP_RecipeDumper.Logger.Log("preTech = \'" + __instance.preTech.Name.Translate() + "\'");
                if (__instance.IconPath.Length > 0)
                    DSP_RecipeDumper.Logger.Log("iconPath = \'" + __instance.IconPath + "\'");
                DSP_RecipeDumper.Logger.Log("isFluid = " + __instance.IsFluid.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("isEntity = " + __instance.IsEntity.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("canBuild = " + __instance.CanBuild.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("buildInGas = " + __instance.BuildInGas.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("isRaw = " + __instance.isRaw.ToString().ToLower());

                if (__instance.prefabDesc != null && __instance.CanBuild && __instance.IsEntity)
                {
                    PrefabDesc p = __instance.prefabDesc;
                    DSP_RecipeDumper.Logger.Log("[prefab." + name + "]");
                    DSP_RecipeDumper.Logger.Log("name = \'" + __instance.name + "\'");

                    // common
                    if (p.minimapType > 0) DSP_RecipeDumper.Logger.Log("minimapType = " + p.minimapType);
                    DSP_RecipeDumper.Logger.Log("hasAudio = " + p.hasAudio.ToString().ToLower());

                    // basic types
                    if (p.veinMiner) DSP_RecipeDumper.Logger.Log("veinMiner = " + p.veinMiner.ToString().ToLower());
                    if (p.oilMiner) DSP_RecipeDumper.Logger.Log("oilMiner = " + p.oilMiner.ToString().ToLower());
                    if (p.isSplitter) DSP_RecipeDumper.Logger.Log("isSplitter = " + p.isSplitter.ToString().ToLower());
                    if (p.isEjector) DSP_RecipeDumper.Logger.Log("isEjector = " + p.isEjector.ToString().ToLower());
                    if (p.isSilo) DSP_RecipeDumper.Logger.Log("isSplitter = " + p.isSilo.ToString().ToLower());
                    if (p.isMonster) DSP_RecipeDumper.Logger.Log("isMonster = " + p.isSilo.ToString().ToLower());

                    // variabled types
                    if (p.isBelt)
                    {
                        DSP_RecipeDumper.Logger.Log("isBelt = " + p.isBelt.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("beltSpeed = " + p.beltSpeed);
                        DSP_RecipeDumper.Logger.Log("beltPrototype = " + p.beltPrototype);
                    }
                    if (p.isStorage)
                    {
                        DSP_RecipeDumper.Logger.Log("isStorage = " + p.isStorage.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("storageCol = " + p.storageCol);
                        DSP_RecipeDumper.Logger.Log("storageRow = " + p.storageRow);
                    }
                    if (p.minerType > 0)
                    {
                        DSP_RecipeDumper.Logger.Log("minerType = \'" + p.minerType + "\'");
                        DSP_RecipeDumper.Logger.Log("minerPeriod = " + p.minerPeriod);
                    }
                    if (p.isInserter)
                    {
                        DSP_RecipeDumper.Logger.Log("isInserter = " + p.isInserter.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("inserterSTT = " + p.inserterSTT);
                        DSP_RecipeDumper.Logger.Log("inserterDelay = " + p.inserterDelay);
                        DSP_RecipeDumper.Logger.Log("inserterCanStack = " + p.inserterCanStack.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("inserterStackSize = " + p.inserterStackSize);
                    }
                    if (p.isAssembler || p.isFractionate)
                    {
                        DSP_RecipeDumper.Logger.Log("isAssembler = " + p.isAssembler.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("assemblerSpeed = " + p.assemblerSpeed);
                        DSP_RecipeDumper.Logger.Log("assemblerRecipeType = \'" + p.assemblerRecipeType + "\'");
                    }
                    if (p.isFractionate)
                    {
                        DSP_RecipeDumper.Logger.Log("isFractionate = " + p.isFractionate.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("assemblerRecipeType = \'" + p.assemblerRecipeType + "\'");
                        DSP_RecipeDumper.Logger.Log("fracNeedMaxCnt = " + p.fracNeedMaxCnt);
                        DSP_RecipeDumper.Logger.Log("fracProductMaxCnt = " + p.fracProductMaxCnt);
                        DSP_RecipeDumper.Logger.Log("fracOriProductMaxCnt = " + p.fracOriProductMaxCnt);
                    }
                    if (p.isLab)
                    {
                        DSP_RecipeDumper.Logger.Log("isLab = " + p.isLab.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("labAssembleSpeed = " + p.labAssembleSpeed);
                        DSP_RecipeDumper.Logger.Log("labResearchSpeed = " + p.labResearchSpeed);
                    }
                    if (p.isTank)
                    {
                        DSP_RecipeDumper.Logger.Log("isTank = " + p.isTank.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("fluidStorageCount = " + p.fluidStorageCount);
                    }
                    if (p.isPowerNode)
                    {
                        DSP_RecipeDumper.Logger.Log("isPowerNode = " + p.isPowerNode.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("powerConnectDistance = " + p.powerConnectDistance);
                        DSP_RecipeDumper.Logger.Log("powerCoverRadius = " + p.powerCoverRadius);
                    }
                    if (p.isPowerGen)
                    {
                        DSP_RecipeDumper.Logger.Log("isPowerGen = " + p.isPowerGen.ToString().ToLower());
                        if (p.photovoltaic) DSP_RecipeDumper.Logger.Log("photovoltaic = " + p.photovoltaic.ToString().ToLower());
                        if (p.windForcedPower) DSP_RecipeDumper.Logger.Log("windForcedPower = " + p.windForcedPower.ToString().ToLower());
                        if (p.gammaRayReceiver) DSP_RecipeDumper.Logger.Log("gammaRayReceiver = " + p.gammaRayReceiver.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("genEnergyPerTick = " + p.genEnergyPerTick);
                        DSP_RecipeDumper.Logger.Log("useFuelPerTick = " + p.useFuelPerTick);
                        DSP_RecipeDumper.Logger.Log("fuelMask = " + p.fuelMask);
                        DSP_RecipeDumper.Logger.Log("powerCatalystId = " + p.powerCatalystId);
                        DSP_RecipeDumper.Logger.Log("powerProductId = " + p.powerProductId);
                        DSP_RecipeDumper.Logger.Log("powerProductHeat = " + p.powerProductHeat);
                    }
                    if (p.isAccumulator)
                    {
                        DSP_RecipeDumper.Logger.Log("isAccumulator = " + p.isAccumulator.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("inputEnergyPerTick = " + p.inputEnergyPerTick);
                        DSP_RecipeDumper.Logger.Log("outputEnergyPerTick = " + p.outputEnergyPerTick);
                        DSP_RecipeDumper.Logger.Log("maxAcuEnergy = " + p.maxAcuEnergy);
                    }
                    if (p.isPowerConsumer || p.isPowerCharger)
                    {
                        if (p.isPowerConsumer) DSP_RecipeDumper.Logger.Log("isPowerConsumer = " + p.isPowerConsumer.ToString().ToLower());
                        if (p.isPowerCharger) DSP_RecipeDumper.Logger.Log("isPowerCharger = " + p.isPowerConsumer.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("workEnergyPerTick = " + p.workEnergyPerTick);
                        DSP_RecipeDumper.Logger.Log("idleEnergyPerTick = " + p.idleEnergyPerTick);
                    }
                    if (p.isPowerExchanger)
                    {
                        DSP_RecipeDumper.Logger.Log("isPowerExchanger = " + p.isPowerExchanger.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("exchangeEnergyPerTick = " + p.exchangeEnergyPerTick);
                        DSP_RecipeDumper.Logger.Log("emptyId = " + p.emptyId);
                        DSP_RecipeDumper.Logger.Log("fullId = " + p.fullId);
                        DSP_RecipeDumper.Logger.Log("maxExcEnergy = " + p.maxExcEnergy);
                    }
                    if (p.isStation || p.isStellarStation)
                    {
                        if (p.isStation) DSP_RecipeDumper.Logger.Log("isStation = " + p.isStation.ToString().ToLower());
                        if (p.isStellarStation) DSP_RecipeDumper.Logger.Log("isStellarStation = " + p.isStellarStation.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("stationMaxItemCount = " + p.stationMaxItemCount);
                        DSP_RecipeDumper.Logger.Log("stationMaxItemKinds = " + p.stationMaxItemKinds);
                        DSP_RecipeDumper.Logger.Log("stationMaxDroneCount = " + p.stationMaxDroneCount);
                        DSP_RecipeDumper.Logger.Log("stationMaxShipCount = " + p.stationMaxShipCount);
                        DSP_RecipeDumper.Logger.Log("stationMaxEnergyAcc = " + p.stationMaxEnergyAcc);
                    }
                    if (p.isCollectStation)
                    {
                        DSP_RecipeDumper.Logger.Log("isCollectStation = " + p.isCollectStation.ToString().ToLower());
                        DSP_RecipeDumper.Logger.Log("stationCollectSpeed = " + p.stationCollectSpeed);
                        // TODO: public int[] stationCollectIds;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(TechProto), "Preload")]
        public class TechProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(TechProto __instance)
            {
                var name = DSP_RecipeDumper.Translate.Get("tech" + __instance.ID);
                DSP_RecipeDumper.Logger.Log("[tech." + name + "]");
                DSP_RecipeDumper.Logger.Log("name = \'" + __instance.name + "\'");
                DSP_RecipeDumper.Logger.Log("description = '''" + __instance.description.Trim() + "'''");
                DSP_RecipeDumper.Logger.Log("isLabTech = " + __instance.IsLabTech.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("hashNeeded = " + __instance.HashNeeded);
                DSP_RecipeDumper.Logger.Log("iconPath = \'" + __instance.IconPath + "\'");
                if (__instance.PreTechs.Length > 0)
                {
                    DSP_RecipeDumper.Logger.Log("preTechs = [");
                    for (int i = 0; i < __instance.PreTechs.Length; i++)
                        DSP_RecipeDumper.Logger.Log("\'" + DSP_RecipeDumper.Translate.Get("tech" + __instance.PreTechs[i]) + "\', ");
                    DSP_RecipeDumper.Logger.Log("]");
                }
                if (__instance.Items.Length > 0)
                {
                    DSP_RecipeDumper.Logger.Log("[tech." + name + ".items]");
                    for (int i = 0; i < __instance.Items.Length; i++)
                        DSP_RecipeDumper.Logger.Log(DSP_RecipeDumper.Translate.Get("item" + __instance.Items[i]) + " = " + __instance.ItemPoints[i]);
                }
                if (__instance.AddItems.Length > 0)
                {
                    DSP_RecipeDumper.Logger.Log("[tech." + name + ".addItems]");
                    for (int i = 0; i < __instance.AddItems.Length; i++)
                        DSP_RecipeDumper.Logger.Log(DSP_RecipeDumper.Translate.Get("item" + __instance.AddItems[i]) + " = " + __instance.AddItemCounts[i]);
                }
            }
        }
    }
}