using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DSP_RecipeDumper
{
    public static class Logger
    {
        static MemoryStream memoryStream = new MemoryStream(4194304);
        static StreamWriter streamWriter = new StreamWriter((Stream)memoryStream);
        static readonly string path;
        static readonly string iconPath;

        static Logger()
        {
            var recipeDumperDir = new StringBuilder(GameConfig.gameDocumentFolder).Append("dsp_recipedumper/").ToString();
            path = recipeDumperDir + "dsp_recipedumper." + GameConfig.gameVersion.ToFullString() + ".toml";
            UnityEngine.Debug.Log("-- recipe dumping to: " + recipeDumperDir);
            if (!Directory.Exists(recipeDumperDir))
                Directory.CreateDirectory(recipeDumperDir);
            iconPath = recipeDumperDir + "icons/";
            if (!Directory.Exists(iconPath))
                Directory.CreateDirectory(iconPath);
        }

        public static void Log(string str)
        {
            streamWriter.WriteLine(str);
        }

        public static void Close()
        {
            if (memoryStream.Length > 0)
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.WriteTo(fileStream);
                    memoryStream.Close();
                }
            } 
        }
    }
}