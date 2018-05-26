using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using WorkHours.Services;
using WorkHours.UWP.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteSetup))]
namespace WorkHours.UWP.Services
{
    public class SQLiteSetup : ISQLiteSetup
    {
        public SQLite.SQLiteConnection GetConnection()
        {
            var fileName = "Database.db3";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);
            return new SQLite.SQLiteConnection(path);
        }
    }
}
