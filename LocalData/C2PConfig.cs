using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SilentOrbit.Pipedrive;

namespace SilentOrbit.LocalData
{
    /// <summary>
    /// Local saved keys
    /// </summary>
    class C2PConfig
    {
        public static C2PConfig Instance { get; private set; } = new C2PConfig();

        public string PipedriveKey { get; set; }
        public string PushbulletKey { get; set; }
        public string PushbulletDevice { get; set; }

        static readonly string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "config.json");

        static C2PConfig()
        {
            if (File.Exists(path))
            {
                var jsonRead = File.ReadAllText(path);
                Instance = JsonConvert.DeserializeObject<C2PConfig>(jsonRead);
            }
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, json);
        }
    }
}