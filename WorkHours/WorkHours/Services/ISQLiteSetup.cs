using System;
using System.Collections.Generic;
using System.Text;

namespace WorkHours.Services
{
    public interface ISQLiteSetup
    {
        SQLite.SQLiteConnection GetConnection();
    }
}
