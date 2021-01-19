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

namespace SilentOrbit.Pushbullet
{
    class PushMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "link";

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("device_iden")]
        public string DeviceIden { get; set; }

    }
}