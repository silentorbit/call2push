using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SilentOrbit.LocalData;

namespace SilentOrbit.Pushbullet
{
    class PushbulletAPI
    {
        public static List<Device> DeviceList()
        {
            var request = System.Net.WebRequest.Create("https://api.pushbullet.com/v2/devices") as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Access-Token", C2PConfig.Instance.PushbulletKey);

            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    var responseJson = reader.ReadToEnd();
                    var devices = JsonConvert.DeserializeObject<DeviceResponse>(responseJson);
                    return devices.devices;
                }
            }
        }

        public static void Push(string url)
        {
            var msg = new PushMessage()
            {
                Title = "Incoming Call",
                //Body = "Example Body 6",
                URL = url,
                DeviceIden = C2PConfig.Instance.PushbulletDevice,
            };

            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));

            var request = System.Net.WebRequest.Create("https://api.pushbullet.com/v2/pushes") as System.Net.HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Access-Token", C2PConfig.Instance.PushbulletKey);

            request.ContentLength = data.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }

            try
            {
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        var responseJson = reader.ReadToEnd();
                        Console.WriteLine(responseJson);
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    var responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                }
                Console.WriteLine(ex);
            }
        }
    }
}