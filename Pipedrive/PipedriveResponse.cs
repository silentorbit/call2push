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

namespace SilentOrbit.Pipedrive
{
    class PipedriveResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public List<Person> Data { get; set; }

        [JsonProperty("additional_data")]
        public AdditionalData AdditionalData { get; set; }

        //[JsonProperty("related_objects")]
        //public object RelatedObjects { get; set; }
    }

    class Person
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("phone")]
        public List<Phone> Phone { get; set; } = new List<Phone>();

        //...more
    }

    class Phone
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        //...more
    }

    class AdditionalData
    {
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }

    class Pagination
    {
        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("more_items_in_collection")]
        public bool MoreItemsInCollection { get; set; }

        [JsonProperty("next_start")]
        public int NextStart { get; set; }

    }
}