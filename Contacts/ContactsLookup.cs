using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Provider.ContactsContract;
using static Android.Provider.ContactsContract.CommonDataKinds;
using Uri = Android.Net.Uri;

namespace SilentOrbit
{
    /// <summary>
    /// Scan Android Contacts for notes with the prefix: "call2push:" or "c2p:"
    /// followed by the url to push
    /// </summary>
    class ContactsLookup
    {
        readonly Context context;

        ContactsLookup(Context context)
        {
            this.context = context;
        }

        /// <summary>
        /// Find notes in contacts with prefix: "call2push:" or "c2p:"
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string Lookup(Context context, string number)
        {
            return new ContactsLookup(context).PhoneLookup(number);
        }

        string DebugColumns(ICursor cur)
        {
            string cols = "";

            var colNames = cur.GetColumnNames();
            for (var i = 0; i < colNames.Length; i++)
            {
                var val = cur.GetString(i);
                cols += colNames[i] + ": " + val + "\n";
            }
            Console.WriteLine(cols);
            return cols;
        }

        string PhoneLookup(string number)
        {
            var contentResolver = context.ContentResolver;
            using (var cursor = contentResolver.Query(
                uri: Uri.WithAppendedPath(ContactsContract.PhoneLookup.ContentFilterUri, Uri.Encode(number)),
                projection: new[] { PhoneLookupColumns.ContactId },
                selection: null,
                selectionArgs: null,
                sortOrder: null))
            {
                while (cursor.MoveToNext())
                {
                    //var cols = DebugColumns(cursor);

                    //Looks like this contactID is the same as raw_contact_id
                    var contactID = cursor.GetString(cursor.GetColumnIndex("contact_id"));
                    var url = GetNoteUrl(contactID);
                    if (url != null)
                        return url;
                }
            }
            return null;
        }

        string GetNoteUrl(string contactID)
        {
            var contentResolver = context.ContentResolver;
            using (var cursor = contentResolver.Query(
                uri: ContactsContract.Data.ContentUri,
                projection: new[] { ContactsContract.DataColumns.Data1 }, //Notes
                selection: "contact_id = ?",
                selectionArgs: new[] { contactID },
                sortOrder: null))
            {
                string note = "";
                while (cursor.MoveToNext())
                {
                    note += cursor.GetString(cursor.GetColumnIndex(ContactsContract.DataColumns.Data1)) + "\n";
                }
                return ParseNote(note);
            }
        }

        static readonly Regex noteLink = new Regex("c(all)?2p(ush)?: *([^ ]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        string ParseNote(string note)
        {
            var lines = note.Split('\n', '\r');
            foreach (var l in lines)
            {
                var m = noteLink.Match(l);
                if (m.Success)
                    return m.Groups[3].Value;
            }

            return null;
        }

    }
}