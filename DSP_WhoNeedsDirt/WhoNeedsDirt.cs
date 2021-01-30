using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using BepInEx;
using System.Reflection.Emit;

namespace DSP_WhoNeedsDirt
{
    [BepInPlugin("us.evilgeni.dsp_whoneedsdirt", "Dirt Override Plug-In", "0.0.1.0")]
    [BepInProcess("DSPGAME.exe")]
    public class DSP_WhoNeedsDirt : BaseUnityPlugin
    {
        internal void Awake()
        {
            var harmony = new Harmony("us.evilgeni.dsp_whoneedsdirt");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(PlanetFactory), "ComputeFlattenTerrainReform")]
        public class PlanetFactory_ComputeFlattenTerrainReform
        {
            public static void Postfix(ref int __result)
            {
                __result = 0;
            }
        }
    }
}