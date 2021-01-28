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
                DSP_RecipeDumper.Logger.Log("version = \"\"\"" + GameConfig.gameVersion.ToFullString() + "\"\"\"");
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
                var name = "recipe" + __instance.ID;
                DSP_RecipeDumper.Logger.Log("[" + name + "]");
                DSP_RecipeDumper.Logger.Log("name = \"\"\"" + __instance.name + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("timespend = " + __instance.TimeSpend);
                DSP_RecipeDumper.Logger.Log("type = \"\"\"" + __instance.Type + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("handcraft = " + __instance.Handcraft.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("explicit = " + __instance.Explicit.ToString().ToLower());
                if (__instance.preTech != (TechProto)null)
                    DSP_RecipeDumper.Logger.Log("preTech = \"\"\"" + __instance.preTech.Name.Translate() + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("iconPath = \"\"\"" + __instance.IconPath + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("[" + name + ".items]");
                for (int i = 0; i < __instance.Items.Length; i++)
                    DSP_RecipeDumper.Logger.Log("item" + __instance.Items[i] + " = " + __instance.ItemCounts[i]);
                DSP_RecipeDumper.Logger.Log("[" + name + ".results]");
                for (int i = 0; i < __instance.Results.Length; i++)
                    DSP_RecipeDumper.Logger.Log("item" + __instance.Results[i] + " = " + __instance.ResultCounts[i]);
            }
        }

        [HarmonyPatch(typeof(ItemProto), "Preload")]
        public class ItemProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(ItemProto __instance)
            {
                var name = "item" + __instance.ID;
                DSP_RecipeDumper.Logger.Log("[" + name + "]");
                DSP_RecipeDumper.Logger.Log("name = \"\"\"" + __instance.name + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("miningFrom = \"\"\"" + __instance.miningFrom + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("produceFrom = \"\"\"" + __instance.produceFrom + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("description = \"\"\"" + __instance.description + "\"\"\"");
                if (__instance.preTech != (TechProto) null)
                    DSP_RecipeDumper.Logger.Log("preTech = \"\"\"" + __instance.preTech.Name.Translate() + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("iconPath = \"\"\"" + __instance.IconPath + "\"\"\"");
            }
        }

        [HarmonyPatch(typeof(TechProto), "Preload")]
        public class TechProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(TechProto __instance)
            {
                var name = "tech" + __instance.ID;
                DSP_RecipeDumper.Logger.Log("[" + name + "]");
                DSP_RecipeDumper.Logger.Log("name = \"\"\"" + __instance.name + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("description = \"\"\"" + __instance.description + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("isLabTech = " + __instance.IsLabTech.ToString().ToLower());
                DSP_RecipeDumper.Logger.Log("hashNeeded = " + __instance.HashNeeded);
                DSP_RecipeDumper.Logger.Log("iconPath = \"\"\"" + __instance.IconPath + "\"\"\"");
                DSP_RecipeDumper.Logger.Log("preTechs = [");
                for (int i = 0; i < __instance.PreTechs.Length; i++)
                    DSP_RecipeDumper.Logger.Log("\"\"\"tech" + __instance.PreTechs[i] + "\"\"\", ");
                DSP_RecipeDumper.Logger.Log("]");
                DSP_RecipeDumper.Logger.Log("[" + name + ".items]");
                for (int i = 0; i < __instance.Items.Length; i++)
                    DSP_RecipeDumper.Logger.Log("item" + __instance.Items[i] + " = " + __instance.ItemPoints[i]);
                DSP_RecipeDumper.Logger.Log("[" + name + ".addItems]");
                for (int i = 0; i < __instance.AddItems.Length; i++)
                    DSP_RecipeDumper.Logger.Log("item" + __instance.AddItems[i] + " = " + __instance.AddItemCounts[i]);
            }
        }
    }
}