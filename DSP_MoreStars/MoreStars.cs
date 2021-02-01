using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection.Emit;

namespace DSP_MoreStars
{
    [BepInPlugin("us.evilgeni.dsp_morestars", "More Stars Plug-In", "0.0.1.0")]
    [BepInProcess("DSPGAME.exe")]
    public class DSP_MoreStars : BaseUnityPlugin
    {
        internal void Awake()
        {
            var harmony = new Harmony("us.evilgeni.dsp_morestars");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(UIGalaxySelect), "_OnInit")]
        public class UIGalaxySelect__OnInit
        {
            public static void Postfix(UIGalaxySelect __instance, ref Slider ___starCountSlider)
            {
                ___starCountSlider.maxValue = 255;
            }
        }

        [HarmonyPatch(typeof(UIGalaxySelect), "OnStarCountSliderValueChange")]
        public static class UIGalaxySelect_OnStarCountSliderValueChange
        {
            //// [196 10 - 196 23]
            //IL_0023: ldloc.0      // num
            //IL_0024: ldc.i4.s     80 // 0x50
            //IL_0026: ble IL_002e

            //// [197 7 - 197 15]
            //IL_002b: ldc.i4.s     80 // 0x50
            //IL_002d: stloc.0      // num

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldloc_0 && codes[i + 1].opcode == OpCodes.Ldc_I4_S && codes[i + 2].opcode == OpCodes.Ble && codes[i + 3].opcode == OpCodes.Ldc_I4_S)
                    {
                        UnityEngine.Debug.Log("DSP_MoreStars: Found signature. Applying patch.");
                        codes[i].opcode     = OpCodes.Nop;
                        codes[i + 1].opcode = OpCodes.Nop;
                        codes[i + 2].opcode = OpCodes.Br;
                        return codes.AsEnumerable();
                    }
                }
                UnityEngine.Debug.Log("DSP_MoreStars: Failed to apply all patches.");
                return codes.AsEnumerable();
            }
        }
    }
}
