using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using BepInEx;

namespace DSP_SeedExporter
{
    [BepInPlugin("us.evilgeni.dsp_seedexporter", "Seed Exporter Plug-In", "0.0.1.0")]
    [BepInProcess("DSPGAME.exe")]
    public class DSP_SeedExporter : BaseUnityPlugin
    {
        internal void Awake()
        {
            var harmony = new Harmony("us.evilgeni.dsp_seedexporter");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(GameMain), "Begin")]
        public class GameMain_Begin
        {
            static string seedExporterDir;
            [HarmonyPostfix]
            public static void Postfix(GameMain __instance)
            {
                seedExporterDir = new StringBuilder(GameConfig.gameDocumentFolder).Append("dsp_seedexporter/").ToString();
                UnityEngine.Debug.Log("-- seed exporting to: " + seedExporterDir);
                if (!Directory.Exists(seedExporterDir))
                    Directory.CreateDirectory(seedExporterDir);

                if (!DSPGame.IsMenuDemo) return;
                exportSeed(69696969);
                exportSeed(8600110);
            }

            public static void exportSeed(int seed)
            {
                UnityEngine.Debug.Log("exportSeed:" + seed);
                // set up memoryStream to write output to
                MemoryStream memoryStream = new MemoryStream();
                StreamWriter streamWriter = new StreamWriter((Stream)memoryStream);
                string path = seedExporterDir + "seed_" + seed + ".json";

                // create new galaxy
                GameDesc gameDesc = new GameDesc();
                gameDesc.SetForNewGame(UniverseGen.algoVersion, seed, 64, 1, 1f);
                GalaxyData galaxy = UniverseGen.CreateGalaxy(gameDesc);
                //UnityEngine.Debug.Log("DSP_SeedExporter: seed: " + galaxy.seed);
                //UnityEngine.Debug.Log("DSP_SeedExporter: starCount: " + galaxy.starCount);
                //UnityEngine.Debug.Log("DSP_SeedExporter: birthPlanetId: " + galaxy.birthPlanetId);
                //UnityEngine.Debug.Log("DSP_SeedExporter: birthStarId: " + galaxy.birthStarId);
                //UnityEngine.Debug.Log("DSP_SeedExporter: habitableCount: " + galaxy.habitableCount);
                //for (int i = 0; i < galaxy.stars.Length; i++)
                //{
                //    UnityEngine.Debug.Log("DSP_SeedExporter: star index: " + galaxy.stars[i].index);
                //    UnityEngine.Debug.Log("DSP_SeedExporter: --          id: " + galaxy.stars[i].id);
                //    UnityEngine.Debug.Log("DSP_SeedExporter: --        name: " + galaxy.stars[i].name);
                //    UnityEngine.Debug.Log("DSP_SeedExporter: --        mass: " + galaxy.stars[i].mass);
                //    UnityEngine.Debug.Log("DSP_SeedExporter: --    lifetime: " + galaxy.stars[i].lifetime);
                //    UnityEngine.Debug.Log("DSP_SeedExporter: --         age: " + galaxy.stars[i].age);
                //    UnityEngine.Debug.Log("DSP_SeedExporter: --        type: " + galaxy.stars[i].type);
                //    UnityEngine.Debug.Log("DSP_SeedExporter: -- temperature: " + galaxy.stars[i].temperature);
                //}
                //UnityEngine.Debug.Log("DSP_SeedExporter: ---");

                // dump to json
                streamWriter.Write("{\"meta\":{" + 
                                   "\"gameVersion\":\"" + GameConfig.gameVersion.ToFullString() + "\"" +
                                   ",\"toolVersion\":\"0.0.1.0\"" + 
                                   ",\"seed\":" + seed + 
                                   ",\"starCount\":" + galaxy.starCount +
                                   ",\"birthPlanetId\":" + galaxy.birthPlanetId +
                                   ",\"birthStarId\":" + galaxy.birthStarId +
                                   ",\"habitableCount\":" + galaxy.habitableCount +
                                   "},"); // opens main dict
                streamWriter.Write("\"star\":{"); // open star dict
                for (int i = 0; i < galaxy.stars.Length; i++)
                {
                    if (i > 0) streamWriter.Write(",");
                    var star = galaxy.stars[i];
                    streamWriter.Write("\"" + star.name + "\":{" +
                        "\"id\":" + star.id +
                        ",\"mass\":" + star.mass +
                        ",\"lifetime\":" + star.lifetime +
                        ",\"age\":" + star.age +
                        ",\"type\":" + (int)star.type +
                        ",\"temperature\":" + star.temperature
                    );
                    streamWriter.Write("}"); // close star dict
                }
                streamWriter.Write("}"); // close star[] dict

                streamWriter.Write("}"); // closes main dict
                // free the galaxy
                galaxy.Free();

                // close the stream, after flushing it.
                streamWriter.Flush();
                if (memoryStream != null && memoryStream.Length > 0)
                {
                    UnityEngine.Debug.Log("DSP_SeedExporter: dumping to: " + path);
                    using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        memoryStream.WriteTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                        streamWriter.Close();
                        memoryStream.Close();
                        memoryStream = null;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Steamworks.SteamAPI), "Init")]
        public class SteamFix
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result)
            {
                __result = true;
            }
        }
    }
}
