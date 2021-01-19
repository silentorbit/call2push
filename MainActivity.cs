using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SilentOrbit.LocalData;
using SilentOrbit.Pushbullet;

namespace SilentOrbit
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        readonly string[] requiredPermissions = new[] {
                Manifest.Permission.ReadContacts, //read url from address book
                Manifest.Permission.ReadPhoneState, //Get incoming calls
                Manifest.Permission.ReadPhoneNumbers, //Get incoming phone number
                Manifest.Permission.ReadCallLog,//Get incoming phone number
            };

        TextView pipedriveStatusView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            FindViewById<Button>(Resource.Id.buttonPipedrive).Click += PipedriveAPI;
            FindViewById<Button>(Resource.Id.buttonPipedriveReload).Click += PipedriveReload;

            FindViewById<Button>(Resource.Id.buttonPushbulletAPI).Click += Click_PushbulletAPI;
            FindViewById<Button>(Resource.Id.buttonPushbulletDevice).Click += Click_PushbulletDevice;
            FindViewById<Button>(Resource.Id.buttonPushbulletTest).Click += Click_PushbulletTest;

            pipedriveStatusView = FindViewById<TextView>(Resource.Id.statusPipedrive);

            foreach (var perm in requiredPermissions)
            {
                if (CheckSelfPermission(perm) == Android.Content.PM.Permission.Denied)
                {
                    RequestPermissions(requiredPermissions, 0);
                }
            }

            try
            {
                var count = PipedriveStorage.Load(PipedriveStorage.LoadMode.Full);
                ShowMessage("Loaded " + count + " contacts from local");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.GetType().Name + ": " + ex.Message);
            }
        }

        void PipedriveAPI(object sender, EventArgs e)
        {
            var c = C2PConfig.Instance;

            Prompt("Pipedrive API Key", PasswordProtect(c.PipedriveKey), (pipeKey) =>
            {
                if (pipeKey.Contains("•"))
                    return;

                c.PipedriveKey = pipeKey;
                c.Save();
            });
        }

        void PipedriveReload(object sender, EventArgs eventArgs)
        {
            try
            {
                var count = PipedriveStorage.Load(PipedriveStorage.LoadMode.Reload);
                ShowMessage("Loaded " + count + " contacts from Pipedrive");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.GetType().Name + ": " + ex.Message);
            }
        }


        void Click_PushbulletAPI(object sender, EventArgs e)
        {
            var c = C2PConfig.Instance;

            Prompt("Pushbullet API Key", PasswordProtect(c.PushbulletKey), (pushKey) =>
            {
                if (pushKey.Contains("•"))
                    return;

                c.PushbulletKey = pushKey;
                c.Save();
            });
        }


        void Click_PushbulletDevice(object sender, EventArgs e)
        {
            var c = C2PConfig.Instance;

            var list = PushbulletAPI.DeviceList().Where(d => d.active && !string.IsNullOrWhiteSpace(d.ToString())).ToArray();
            var items = list.Select(d => d.nickname + " " + d.model + " " + d.manufacturer).ToArray();

            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Pushbullet Device");
            builder.SetItems(items, (object sender, DialogClickEventArgs e) =>
            {
                var d = list[e.Which];
                c.PushbulletDevice = d.iden;
                c.Save();
            });
            builder.Create().Show();
        }

        void Click_PushbulletTest(object sender, EventArgs e)
        {
            PushbulletAPI.Push("https://www.silentorbit.com/call2push/");
        }


        static string PasswordProtect(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "";
            else
                return "••••••";
        }

        void ShowMessage(string message)
        {
            Toast.MakeText(ApplicationContext, message, ToastLength.Long).Show();
            pipedriveStatusView.Text = message;
        }

        void Prompt(string title, string value, Action<string> save)
        {
            var builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle(title);
            var text = new EditText(this);
            text.InputType = Android.Text.InputTypes.ClassText;
            text.Text = value;
            builder.SetView(text);
            Android.App.AlertDialog dialog;
            builder.SetPositiveButton("OK", (s, e) =>
            {
                save(text.Text);
            });
            builder.SetNegativeButton("Cancel", (s, e) =>
            {
                ((Android.App.AlertDialog)s).Cancel();
            });
            dialog = builder.Show();
        }
    }
}

