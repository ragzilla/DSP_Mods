using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            static MemoryStream memoryStream;
            static StreamWriter streamWriter;
            static bool firstPlanet;
            static object __lock = new object();

            static string seedExporterDir;
            [HarmonyPostfix]
            public static void Postfix(GameMain __instance)
            {
                seedExporterDir = new StringBuilder(GameConfig.gameDocumentFolder).Append("dsp_seedexporter/").ToString();
                UnityEngine.Debug.Log("-- seed exporting to: " + seedExporterDir);
                if (!Directory.Exists(seedExporterDir))
                    Directory.CreateDirectory(seedExporterDir);

                if (!DSPGame.IsMenuDemo) return;
                //exportSeed(69696969);
                //exportSeed(8600110);
                //return;
                var random = new System.Random((int)(DateTime.Now.Ticks / 10000L));
                while (true)
                {
                    exportSeed(random.Next(100000000));
                }
            }

            public static void exportSeed(int seed)
            {
                UnityEngine.Debug.Log("exportSeed:" + seed);
                // set up memoryStream to write output to
                memoryStream = new MemoryStream();
                streamWriter = new StreamWriter((Stream)memoryStream);
                string path = seedExporterDir + "seed_" + seed + ".json";

                // create new galaxy
                GameDesc gameDesc = new GameDesc();
                gameDesc.SetForNewGame(UniverseGen.algoVersion, seed, 64, 1, 1f);
                GalaxyData galaxy = UniverseGen.CreateGalaxy(gameDesc);

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
                        "\"seed\":" + star.seed +
                        ",\"index\":" + star.index +
                        ",\"id\":" + star.id +
                        ",\"name\":\"" + star.name + "\"" +
                        ",\"position\":{\"x\":" + star.position.x + ",\"y\":" + star.position.y + ",\"z\":" + star.position.z + "}" +
                        ",\"uPosition\":{\"x\":" + star.uPosition.x + ",\"y\":" + star.uPosition.y + ",\"z\":" + star.uPosition.z + "}" +
                        ",\"mass\":" + star.mass +
                        ",\"lifetime\":" + star.lifetime +
                        ",\"age\":" + star.age +
                        ",\"type\":" + (int)star.type +
                        ",\"temperature\":" + star.temperature +
                        ",\"spectr\":" + (int)star.spectr +
                        ",\"classFactor\":" + star.classFactor +
                        ",\"color\":" + star.color +
                        ",\"luminosity\":" + star.luminosity +
                        ",\"radius\":" + star.radius +
                        ",\"acdiskRadius\":" + star.acdiskRadius +
                        ",\"habitableRadius\":" + star.habitableRadius +
                        ",\"lightBalanceRadius\":" + star.lightBalanceRadius +
                        ",\"dysonRadius\":" + star.dysonRadius +
                        ",\"orbitScaler\":" + star.orbitScaler +
                        ",\"asterBelt1OrbitIndex\":" + star.asterBelt1OrbitIndex +
                        ",\"asterBelt2OrbitIndex\":" + star.asterBelt2OrbitIndex +
                        ",\"asterBelt1Radius\":" + star.asterBelt1Radius +
                        ",\"asterBelt2Radius\":" + star.asterBelt2Radius +
                        ",\"planetCount\":" + star.planetCount +
                        ",\"level\":" + star.level +
                        ",\"resourceCoef\":" + star.resourceCoef +
                        ",\"planet\":{"
                    ); // initial data, open planet[]
                    firstPlanet = true;
                    for (int j = 0; j < star.planets.Length; j++)
                    {
                        exportStar(star.planets[j]);
                    }
                    streamWriter.Write("}}"); // close planet[] and star dicts
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

            private static void exportStar(PlanetData planet)
            {
                UnityEngine.Debug.Log("-- planet: " + planet.id);
                PlanetAlgorithm planetAlgorithm = PlanetModelingManager.Algorithm(planet);
                planet.data = new PlanetRawData(planet.precision);
                planet.modData = planet.data.InitModData(planet.modData);
                planet.data.CalcVerts();
                planet.aux = new PlanetAuxData(planet);
                planetAlgorithm.GenerateTerrain(planet.mod_x, planet.mod_y);
                planetAlgorithm.CalcWaterPercent();
                if (planet.type != EPlanetType.Gas)
                    planetAlgorithm.GenerateVegetables();
                if (planet.type != EPlanetType.Gas)
                    planetAlgorithm.GenerateVeins(false);
                Monitor.Enter(streamWriter);
                try
                {
                    if (!firstPlanet) { streamWriter.Write(","); } else { firstPlanet = false; }
                    streamWriter.Write("\"" + planet.name + "\":{" +
                        "\"seed\":" + planet.seed +
                        ",\"index\":" + planet.index +
                        ",\"id\":" + planet.id +
                        ",\"orbitAround\":" + planet.orbitAround +
                        ",\"number\":" + planet.number +
                        ",\"orbitIndex\":" + planet.orbitIndex +
                        ",\"name\":\"" + planet.name + "\"" +
                        ",\"orbitRadius\":" + planet.orbitRadius +
                        ",\"orbitInclination\":" + planet.orbitInclination +
                        ",\"orbitLongitude\":" + planet.orbitLongitude +
                        ",\"orbitalPeriod\":" + planet.orbitalPeriod +
                        ",\"orbitPhase\":" + planet.orbitPhase +
                        ",\"obliquity\":" + planet.obliquity +
                        ",\"rotationPeriod\":" + planet.rotationPeriod +
                        ",\"rotationPhase\":" + planet.rotationPhase +
                        ",\"radius\":" + planet.radius +
                        ",\"scale\":" + planet.scale +
                        ",\"sunDistance\":" + planet.sunDistance +
                        ",\"habitableBias\":" + planet.habitableBias +
                        ",\"temperatureBias\":" + planet.temperatureBias +
                        ",\"ionHeight\":" + planet.ionHeight +
                        ",\"windStrength\":" + planet.windStrength +
                        ",\"luminosity\":" + planet.luminosity +
                        ",\"landPercent\":" + planet.landPercent +
                        ",\"mod_x\":" + planet.mod_x +
                        ",\"mod_y\":" + planet.mod_y +
                        ",\"type\":" + (int)planet.type +
                        ",\"singularity\":" + (int)planet.singularity +
                        ",\"theme\":" + planet.theme +
                        ",\"algoId\":" + planet.algoId +
                        ",\"waterHeight\":" + planet.waterHeight);
                    if (planet.waterItemId > 0)
                    {
                        ItemProto water = LDB.items.Select(planet.waterItemId);
                        streamWriter.Write(
                            ",\"waterItem\":\"" + water.name + "\""
                        );
                    }
                    if (planet.type == EPlanetType.Gas)
                    {
                        streamWriter.Write(",\"gasTotalHeat\":" + planet.gasTotalHeat);
                        streamWriter.Write(",\"gas\":{"); // open gas[]
                        bool firstvein = true;
                        for (int k = 0; k < planet.gasItems.Length; k++)
                        {
                            ItemProto gas = LDB.items.Select(planet.gasItems[k]);
                            if (firstvein == false) streamWriter.Write(","); else firstvein = false;
                            streamWriter.Write("\"" + gas.name + "\":{" +
                                "\"gasName\":\"" + gas.name + "\"" +
                                ",\"gasItem\":" + planet.gasItems[k] +
                                ",\"gasSpeed\":\"" + planet.gasSpeeds[k] + "\"" +
                                ",\"gasHeatValue\":\"" + planet.gasHeatValues[k] + "\"}");
                        }
                        streamWriter.Write("}"); // close gas[]
                    }
                    else
                    {
                        streamWriter.Write(",\"vein\":{"); // open vein[]
                        bool firstvein = true;
                        for (int k = 0; k < planet.veinAmounts.Length; k++)
                        {
                            if (planet.veinAmounts[k] == 0) continue;
                            if (firstvein == false) streamWriter.Write(","); else firstvein = false;
                            streamWriter.Write("\"" + (EVeinType)k + "\":" + planet.veinAmounts[k]);
                        }
                        streamWriter.Write("}"); // close vein[]
                    }
                    streamWriter.Write(",\"uPosition\":{\"x\":" + planet.uPosition.x + ",\"y\":" + planet.uPosition.y + ",\"z\":" + planet.uPosition.z + "}}"); // close planet
                }
                finally { Monitor.Exit(streamWriter); }
                planet.Unload();
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
