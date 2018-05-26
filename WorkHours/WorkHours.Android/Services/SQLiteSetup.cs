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
using WorkHours.Services;
using SQLite;
using System.IO;
using Xamarin.Forms;
using WorkHours.Droid.Services;

[assembly: Dependency(typeof(SQLiteSetup))]
namespace WorkHours.Droid.Services
{
    public class SQLiteSetup : ISQLiteSetup
    {
        public SQLiteConnection GetConnection()
        {
            var fileName = "WorkHoursDatabase.db3";
            var dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(dbPath, fileName);
            return new SQLiteConnection(path);
        }
    }
}