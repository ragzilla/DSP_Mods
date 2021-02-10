using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using BepInEx;

namespace DSP_ColorExport
{
    [BepInPlugin("us.evilgeni.dsp_colorexport", "Color Export Plug-In", "0.0.1.0")]
    [BepInProcess("DSPGAME.exe")]
    public class DSP_ColorExportPlugin : BaseUnityPlugin
    {
        internal void Awake()
        {
            var harmony = new Harmony("us.evilgeni.dsp_colorexport");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(UIVirtualStarmap), "_OnCreate")]
        public class UIVirtualStarmap__OnCreate
        {
            public static void Postfix(UIVirtualStarmap __instance)
            {
                UnityEngine.Debug.Log("In UIVirtualStarmap::_OnCreate()");
                UnityEngine.Debug.Log("neutronStarColor = " + __instance.neutronStarColor);
                UnityEngine.Debug.Log("whiteDwarfColor = " + __instance.whiteDwarfColor);
                UnityEngine.Debug.Log("blackholeColor = " + __instance.blackholeColor);
                for (int i = 0; i < __instance.starColors.colorKeys.Length; i++)
                    UnityEngine.Debug.Log("colorKey[" + i + "]: " + __instance.starColors.colorKeys[i].color + " @ " + __instance.starColors.colorKeys[i].time);
                for (int i = 0; i < __instance.starColors.alphaKeys.Length; i++)
                    UnityEngine.Debug.Log("alphaKey[" + i  + "]: " + __instance.starColors.alphaKeys[i].alpha + " @ " + __instance.starColors.alphaKeys[i].time);
            }
        }
    }
}