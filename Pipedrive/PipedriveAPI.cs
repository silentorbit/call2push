using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SilentOrbit.LocalData;

namespace SilentOrbit.Pipedrive
{
    static class PipedriveAPI
    {
        static Dictionary<string, int> phone2ID = new Dictionary<string, int>();

        public static Dictionary<string, int> LoadAllPersons()
        {
            phone2ID.Clear();

            int start = 0;
            int limit = 500;
            while (true)
            {
                var resp = GetPersons(start, limit);

                if (resp.Success == false)
                    throw new NotImplementedException();
                var page = resp.AdditionalData.Pagination;
                if (page.Start != start)
                    throw new NotImplementedException();
                if (page.Limit != limit)
                    throw new NotImplementedException();
                if (page.MoreItemsInCollection && page.NextStart != start + limit)
                    throw new NotImplementedException();

                foreach (var p in resp.Data)
                {
                    foreach (var ph in p.Phone)
                    {
                        AddPhone(p.ID, ph.Value);
                    }
                }

                if (page.MoreItemsInCollection)
                {
                    start = page.NextStart;
                    continue;
                }
                else
                {
                    break;
                }
            }

            return phone2ID;
        }


        static void AddPhone(int id, string value)
        {
            value = PipedriveStorage.Normalize(value);

            if (value == null)
                return;

            phone2ID.TryAdd(value, id);
        }

        static PipedriveResponse GetPersons(int start, int limit)
        {
            if (limit > 500)
                throw new ArgumentOutOfRangeException(nameof(limit));


            string url = "https://murbox.pipedrive.com/v1/persons?api_token=" + WebUtility.UrlEncode(C2PConfig.Instance.PipedriveKey) + "&start=" + start + "&limit=" + limit;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                using (var response = request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var respJson = reader.ReadToEnd();
                    var resp = JsonConvert.DeserializeObject<PipedriveResponse>(respJson);
                    return resp;
                }
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    using (var reader = new StreamReader(we.Response.GetResponseStream()))
                    {
                        var resp = reader.ReadToEnd();
                        throw new Exception(we.Message + "\n" + url + "\n" + resp, we);
                    }
                }
                else
                {
                    throw new Exception(we.Message + "\n" + url + "\n" + we.Status, we);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + url, ex);
            }
        }
    }
}