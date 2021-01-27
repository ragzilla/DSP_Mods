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

        [HarmonyPatch(typeof(RecipeProto), "Preload")]
        public class RecipeProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(RecipeProto __instance)
            {
                UnityEngine.Debug.Log("[recipe" + __instance.ID + "]");
                // UnityEngine.Debug.Log("sid = \"" + __instance.SID + "\"");
                UnityEngine.Debug.Log("name = \"" + __instance.name + "\"");
                UnityEngine.Debug.Log("timespend = " + __instance.TimeSpend);
                UnityEngine.Debug.Log("type = " + __instance.Type);
                UnityEngine.Debug.Log("handcraft = " + __instance.Handcraft);
                UnityEngine.Debug.Log("explicit = " + __instance.Explicit);
                if (__instance.preTech != (TechProto)null)
                    UnityEngine.Debug.Log("preTech = \"" + __instance.preTech.Name.Translate() + "\"");
                UnityEngine.Debug.Log("iconPath = \"" + __instance.IconPath + "\"");
                UnityEngine.Debug.Log("items = [");
                for (int i = 0; i < __instance.Items.Length; i++)
                    UnityEngine.Debug.Log("item" + __instance.Items[i] + " = " + __instance.ItemCounts[i]);
                UnityEngine.Debug.Log("]");
                UnityEngine.Debug.Log("results = [");
                for (int i = 0; i < __instance.Results.Length; i++)
                    UnityEngine.Debug.Log("item" + __instance.Results[i] + " = " + __instance.ResultCounts[i]);
                UnityEngine.Debug.Log("]");
            }
        }

        [HarmonyPatch(typeof(ItemProto), "Preload")]
        public class ItemProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(ItemProto __instance)
            {
                UnityEngine.Debug.Log("[item" + __instance.ID + "]");
                UnityEngine.Debug.Log("name = \"" + __instance.name + "\"");
                UnityEngine.Debug.Log("miningFrom = \"" + __instance.miningFrom + "\"");
                UnityEngine.Debug.Log("produceFrom = \"" + __instance.produceFrom + "\"");
                UnityEngine.Debug.Log("description = \"" + __instance.description + "\"");
                if (__instance.preTech != (TechProto) null)
                    UnityEngine.Debug.Log("preTech = \"" + __instance.preTech.Name.Translate() + "\"");
                UnityEngine.Debug.Log("iconPath = \"" + __instance.IconPath + "\"");
            }
        }
    }
}