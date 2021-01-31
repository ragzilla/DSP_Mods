using HarmonyLib;
using BepInEx;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace DSP_NoResearchNag
{
    [BepInPlugin("us.evilgeni.dsp_noresearchnag", "Disable Research Nag Plug-In", "0.0.1.0")]
    [BepInProcess("DSPGAME.exe")]
    public class DSP_NoResearchNag : BaseUnityPlugin
    {
        internal void Awake()
        {
            var harmony = new Harmony("us.evilgeni.dsp_noresearchnag");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(GameTutorialLogic), "GameTick")]
        public class GameTutorialLogic_GameTick
        {
            // // [61 5 - 61 39]
            // IL_0129: ldarg.0      // this
            // IL_012a: ldfld int32 GameTutorialLogic::lowResearchSpeedPops
            // IL_012f: ldc.i4.3
            // IL_0130: bge IL_01f4

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldarg_0 && codes[i + 1].opcode == OpCodes.Ldfld && codes[i + 2].opcode == OpCodes.Ldc_I4_3 && codes[i + 3].opcode == OpCodes.Bge)
                    {
                        // change comparison from < 3 to >= 0
                        UnityEngine.Debug.Log("DSP_NoResearchNag: Found research nag signature, patching");
                        codes[i + 2].opcode = OpCodes.Ldc_I4_0;
                        codes[i + 3].opcode = OpCodes.Bge;
                        return codes.AsEnumerable();
                    }
                }
                UnityEngine.Debug.Log("DSP_NoResearchNag: Failed to apply patch.");
                return codes.AsEnumerable();
            }

        }
    }
}
