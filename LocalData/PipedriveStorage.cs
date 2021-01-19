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
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SilentOrbit.Pipedrive;

namespace SilentOrbit.LocalData
{
    class PipedriveStorage
    {
        static PipedriveStorage instance;

        public Dictionary<string, int> Phone2ID { get; set; } = new Dictionary<string, int>();

        static readonly string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "contacts.json");

        public enum LoadMode
        {
            /// <summary>
            /// Load from disk only
            /// </summary>
            Quick,
            /// <summary>
            /// Load from disk or internet
            /// </summary>
            Full,
            /// <summary>
            /// Reload from internet
            /// </summary>
            Reload,
        }

        static Mutex mutex = new Mutex();

        public static int Load(LoadMode mode)
        {
            if (mutex.WaitOne(1))
            {
                try
                {
                    LoadInternal(mode);
                    return instance.Phone2ID.Count;
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
                return -1;
        }

        static void LoadInternal(LoadMode mode)
        {
            if (mode != LoadMode.Reload)
            {
                if (instance != null)
                    return;

                if (File.Exists(path))
                {
                    var jsonRead = File.ReadAllText(path);
                    instance = JsonConvert.DeserializeObject<PipedriveStorage>(jsonRead);

                    if (instance.Phone2ID != null)
                        return;
                }
            }

            if (mode == LoadMode.Quick)
                return;

            //Load from API
            var inst = new PipedriveStorage();
            inst.Phone2ID = PipedriveAPI.LoadAllPersons();
            instance = inst;

            //Save
            var jsonWrite = JsonConvert.SerializeObject(instance);
            File.WriteAllText(path, jsonWrite);
        }

        public static string Lookup(string key)
        {
            try
            {
                Load(LoadMode.Quick);
            }
            catch
            {
                
            }

            if (instance == null)
                return null;

            var number = Normalize(key);
            if (number == null)
                return null;

            if (instance.Phone2ID.TryGetValue(number, out var id))
                return "https://murbox.pipedrive.com/person/" + id;
            else
                return null;
        }

        static readonly Regex notNumbers = new Regex("[^\\+\\d]", RegexOptions.Compiled);

        public static string Normalize(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var number = notNumbers.Replace(value, "");

            if (number == "")
                return number;

            if (number.StartsWith("+"))
            {
                return number;
            }
            else
            {
                //Missing country prefix
                //Assume +46

                if (number.StartsWith("0"))
                {
                    return "+46" + number.Substring(1);
                }
                else
                {
                    if (number.Length < 8)
                        return number;

                    return "+46" + number;
                }
            }

        }

    }
}