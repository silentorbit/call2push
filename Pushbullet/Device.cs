using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilentOrbit.Pushbullet
{
    class Device
    {
        public bool active { get; set; }
        public long app_version { get; set; }
        public double created { get; set; }
        public string iden { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public double modified { get; set; }
        public string nickname { get; set; }
        public string push_token { get; set; }

        public override string ToString() => nickname + " " + model + " " + manufacturer;
    }
}