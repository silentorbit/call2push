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

namespace Call2Push
{
    class PushBullet
    {
        public static void Push()
        {
            
            // Push a link to all devices.
            String apiKey = "o.Ub3Gx25SCKaZVYY7qn2fanz8M8e8Ddjb"; //Access token?

            String type = "link", title = "Example Title", body = "Example Body", url = "https://www.murbox.com";
            byte[] data = Encoding.ASCII.GetBytes(String.Format("{{ \"type\": \"{0}\", \"title\": \"{1}\", \"body\": \"{2}\", \"url\": \"{3}\" }}", type, title, body, url));

            var request = System.Net.WebRequest.Create("https://api.pushbullet.com/v2/pushes") as System.Net.HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            //request.Credentials = new System.Net.NetworkCredential(apiKey, "");
            request.Headers.Add("Access-Token", apiKey);

            request.ContentLength = data.Length;
            String responseJson = null;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }

            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    responseJson = reader.ReadToEnd();
                }
            }
        }
    }
}