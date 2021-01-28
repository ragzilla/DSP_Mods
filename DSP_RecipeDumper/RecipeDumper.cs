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
            //UnityEngine.Debug.Log("[meta]");
            //UnityEngine.Debug.Log("version = \"\"\"" + GameConfig.gameVersion.ToFullString() + "\"\"\"");
            //UnityEngine.Debug.Log("build = " + GameConfig.build);
        }

        [HarmonyPatch(typeof(GlobalObject), "LoadVersions")]
        public class GlobalObject_LoadVersions
        {
            public static void Postfix(GlobalObject __instance)
            {
                UnityEngine.Debug.Log("[meta]");
                UnityEngine.Debug.Log("version = \"\"\"" + GameConfig.gameVersion.ToFullString() + "\"\"\"");
                UnityEngine.Debug.Log("build = " + GameConfig.build);
            }
        }

        [HarmonyPatch(typeof(RecipeProto), "Preload")]
        public class RecipeProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(RecipeProto __instance)
            {
                var name = "recipe" + __instance.ID;
                UnityEngine.Debug.Log("[" + name + "]");
                // UnityEngine.Debug.Log("sid = \"\"\"" + __instance.SID + "\"\"\"");
                UnityEngine.Debug.Log("name = \"\"\"" + __instance.name + "\"\"\"");
                UnityEngine.Debug.Log("timespend = " + __instance.TimeSpend);
                UnityEngine.Debug.Log("type = \"\"\"" + __instance.Type + "\"\"\"");
                UnityEngine.Debug.Log("handcraft = " + __instance.Handcraft.ToString().ToLower());
                UnityEngine.Debug.Log("explicit = " + __instance.Explicit.ToString().ToLower());
                if (__instance.preTech != (TechProto)null)
                    UnityEngine.Debug.Log("preTech = \"\"\"" + __instance.preTech.Name.Translate() + "\"\"\"");
                UnityEngine.Debug.Log("iconPath = \"\"\"" + __instance.IconPath + "\"\"\"");
                UnityEngine.Debug.Log("[" + name + ".items]");
                for (int i = 0; i < __instance.Items.Length; i++)
                    UnityEngine.Debug.Log("item" + __instance.Items[i] + " = " + __instance.ItemCounts[i]);
                UnityEngine.Debug.Log("[" + name + ".results]");
                for (int i = 0; i < __instance.Results.Length; i++)
                    UnityEngine.Debug.Log("item" + __instance.Results[i] + " = " + __instance.ResultCounts[i]);
            }
        }

        [HarmonyPatch(typeof(ItemProto), "Preload")]
        public class ItemProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(ItemProto __instance)
            {
                var name = "item" + __instance.ID;
                UnityEngine.Debug.Log("[" + name + "]");
                UnityEngine.Debug.Log("name = \"\"\"" + __instance.name + "\"\"\"");
                UnityEngine.Debug.Log("miningFrom = \"\"\"" + __instance.miningFrom + "\"\"\"");
                UnityEngine.Debug.Log("produceFrom = \"\"\"" + __instance.produceFrom + "\"\"\"");
                UnityEngine.Debug.Log("description = \"\"\"" + __instance.description + "\"\"\"");
                if (__instance.preTech != (TechProto) null)
                    UnityEngine.Debug.Log("preTech = \"\"\"" + __instance.preTech.Name.Translate() + "\"\"\"");
                UnityEngine.Debug.Log("iconPath = \"\"\"" + __instance.IconPath + "\"\"\"");
            }
        }

        [HarmonyPatch(typeof(TechProto), "Preload")]
        public class TechProto_Preload
        {
            [HarmonyPostfix]
            public static void Postfix(TechProto __instance)
            {
                var name = "tech" + __instance.ID;
                UnityEngine.Debug.Log("[" + name + "]");
                UnityEngine.Debug.Log("name = \"\"\"" + __instance.name + "\"\"\"");
                UnityEngine.Debug.Log("description = \"\"\"" + __instance.description + "\"\"\"");
                UnityEngine.Debug.Log("isLabTech = " + __instance.IsLabTech.ToString().ToLower());
                UnityEngine.Debug.Log("hashNeeded = " + __instance.HashNeeded);
                UnityEngine.Debug.Log("iconPath = \"\"\"" + __instance.IconPath + "\"\"\"");
                UnityEngine.Debug.Log("preTechs = [");
                for (int i = 0; i < __instance.PreTechs.Length; i++)
                    UnityEngine.Debug.Log("\"\"\"tech" + __instance.PreTechs[i] + "\"\"\", ");
                UnityEngine.Debug.Log("]");
                UnityEngine.Debug.Log("[" + name + ".items]");
                for (int i = 0; i < __instance.Items.Length; i++)
                    UnityEngine.Debug.Log("item" + __instance.Items[i] + " = " + __instance.ItemPoints[i]);
                UnityEngine.Debug.Log("[" + name + ".addItems]");
                for (int i = 0; i < __instance.AddItems.Length; i++)
                    UnityEngine.Debug.Log("item" + __instance.AddItems[i] + " = " + __instance.AddItemCounts[i]);
            }
        }
    }
}

//public static string gameDocumentFolder
//{
//    get
//    {
//        if (GameConfig.gameDocument == null)
//        {
//            GameConfig.gameDocument = new StringBuilder(GameConfig.overrideDocumentFolder).Append(GameConfig.gameName).Append("/").ToString();
//            if (!Directory.Exists(GameConfig.gameDocument))
//                Directory.CreateDirectory(GameConfig.gameDocument);
//        }
//        return GameConfig.gameDocument;
//    }
//}

//string path = GameConfig.gameSaveFolder + saveName + GameSave.saveExt;
//try
//{
//    using (MemoryStream memoryStream = new MemoryStream(4194304))
//    {
//        using (BinaryWriter w = new BinaryWriter((Stream)memoryStream))
//        {
//using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
//    memoryStream.WriteTo((Stream)fileStream);