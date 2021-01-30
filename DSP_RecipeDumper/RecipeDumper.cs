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
                if (__instance.preTech != (TechProto) null)
                    DSP_RecipeDumper.Logger.Log("preTech = \'" + __instance.preTech.Name.Translate() + "\'");
                if (__instance.IconPath.Length > 0)
                    DSP_RecipeDumper.Logger.Log("iconPath = \'" + __instance.IconPath + "\'");
                DSP_RecipeDumper.Logger.Log("isFluid = " + __instance.IsFluid.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("isEntity = " + __instance.IsEntity.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("canBuild = " + __instance.CanBuild.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("buildInGas = " + __instance.BuildInGas.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("isRaw = " + __instance.isRaw.ToString().ToLower());
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