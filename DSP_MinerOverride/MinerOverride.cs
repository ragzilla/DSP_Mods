using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using BepInEx;
using System.Reflection.Emit;

namespace DSP_MinerOverride
{
    [BepInPlugin("us.evilgeni.dsp_mineroverride", "Miner Override Plug-In", "0.0.1.0")]
    [BepInProcess("DSPGAME.exe")]
    public class DSP_MinerOverridePlugin : BaseUnityPlugin
    {
        internal void Awake()
        {
            var harmony = new Harmony("us.evilgeni.dsp_mineroverride");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(MinerComponent), "InternalUpdate")]
        public static class MinerComponent_InternalUpdate
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                //    // [264 15 - 264 24]
                //    IL_01c7: ldloc.2      // flag
                //    IL_01c8: brfalse IL_0309

                //    // [266 17 - 266 40]
                //    IL_01cd: ldarg.2      // veinPool
                //    IL_01ce: ldloc.1      // vein
                //    IL_01cf: ldelema VeinData
                //    IL_01d4: dup
                //    IL_01d5: ldfld int32 VeinData::amount
                //    IL_01da: ldc.i4.1
                //    IL_01db: sub
                //    IL_01dc: stfld int32 VeinData::amount

                //    IL_0227: ldind.i8
                //    IL_0228: ldc.i4.1
                //    IL_0229: conv.i8
                //    IL_022a: sub
                //    IL_022b: stind.i8

                //    IL_0249: ldfld int64 PlanetData / VeinGroup::amount
                //    IL_024e: ldc.i4.1
                //    IL_024f: conv.i8
                //    IL_0250: sub
                //    IL_0251: stfld int64 PlanetData / VeinGroup::amount

                var phase = 0;

                for (var i = 0; i < codes.Count; i++)
                {
                    if (phase == 0 && codes[i].opcode == OpCodes.Ldfld && codes[i + 1].opcode == OpCodes.Ldc_I4_1 && codes[i + 2].opcode == OpCodes.Sub && codes[i + 3].opcode == OpCodes.Stfld && codes[i - 6].opcode == OpCodes.Ldloc_2 && codes[i - 5].opcode == OpCodes.Brfalse)
                    {
                        // found a load field, followed by decrement and store field, 6 codes back loads local 2 (flag) and jumps to bypass if false, this subtract is the miner subtracting from the Vein
                        codes[i + 1].opcode = OpCodes.Nop; // Ldc_I4_1
                        codes[i + 2].opcode = OpCodes.Nop; // Sub
                        phase++;
                        UnityEngine.Debug.Log("Finished Phase 0 Patch");
                    }
                    if (phase == 1 && codes[i].opcode == OpCodes.Ldind_I8 && codes[i + 1].opcode == OpCodes.Ldc_I4_1 && codes[i + 2].opcode == OpCodes.Conv_I8 && codes[i + 3].opcode == OpCodes.Sub && codes[i + 4].opcode == OpCodes.Stind_I8)
                    {
                        // found a load indirect, followed by decrement and store, this subtracts from factory.planet.veinAmounts
                        codes[i + 1].opcode = OpCodes.Nop; // Ldc_I4_1
                        codes[i + 2].opcode = OpCodes.Nop; // Conv_I8
                        codes[i + 3].opcode = OpCodes.Nop; // Sub
                        phase++;
                        UnityEngine.Debug.Log("Finished Phase 1 Patch");
                    }
                    if (phase == 2 && codes[i].opcode == OpCodes.Ldfld && codes[i + 1].opcode == OpCodes.Ldc_I4_1 && codes[i + 2].opcode == OpCodes.Conv_I8 && codes[i + 3].opcode == OpCodes.Sub && codes[i+4].opcode == OpCodes.Stfld) {
                        // found a load field, followed by decrement and store, this subtracts from factory.planet.veinGroups
                        codes[i + 1].opcode = OpCodes.Nop; // Ldc_I4_1
                        codes[i + 2].opcode = OpCodes.Nop; // Conv_I8
                        codes[i + 3].opcode = OpCodes.Nop; // Sub
                        phase++;
                        UnityEngine.Debug.Log("Finished Phase 2 Patch");
                    }
                    if (phase == 3) return codes.AsEnumerable();
                }
                UnityEngine.Debug.Log("Failed to apply all patches.");
                return codes.AsEnumerable();
            }
        }
    }
}