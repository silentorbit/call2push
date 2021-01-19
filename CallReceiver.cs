using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using SilentOrbit.LocalData;
using SilentOrbit.Pipedrive;
using SilentOrbit.Pushbullet;

namespace SilentOrbit
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "android.intent.action.PHONE_STATE" })]
    public class CallReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var ts = new ContextWrapper(context).GetSystemService(Context.TelephonyService);
            var tm = (TelephonyManager)ts;

            if (tm.CallState != CallState.Ringing)
                return;

            string number = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);

            if (string.IsNullOrEmpty(number))
                return;

            //Remove ending "***" meaning phone number was to the main number
            number = number.TrimEnd('*');

            number = PipedriveStorage.Normalize(number);

            var url = FindUrl(context, number);
            if (url != null)
                PushbulletAPI.Push(url);
        }

        string FindUrl(Context context, string number)
        {
            //Android Contacts: note with prefix: "call2push:" or "c2p:"
            var url = ContactsLookup.Lookup(context, number);
            if (url != null)
                return url;

            //Pipedrive
            url = PipedriveStorage.Lookup(number);
            if (url != null)
                return url;


            return null;
        }
    }
}