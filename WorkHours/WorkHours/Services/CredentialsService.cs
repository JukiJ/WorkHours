using WorkHours.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace WorkHours.Services
{
    class CredentialsService:ICredentialsService
    {
        private SQLite.SQLiteConnection database;
        public CredentialsService()
        {
            try
            {
                database = DependencyService.Get<ISQLiteSetup>().GetConnection();
                database.CreateTable<Person>();
                database.CreateTable<Verification>();
                if (database.Table<Person>().Count() == 0)
                {
                    Person demo = new Person() { Name = "John", Surname = "Doe", Admin = true, Position = "Manager" };
                    database.Insert(demo);
                    database.Insert(new Verification() { Username = "Test", Password = "test", PersonId = demo.Id });
                }
                database.CreateTable<CheckInOut>();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error instantiating connection to the database!! {e.Message}");
            }
        }
        public int CheckCredentials(string username, string password)
        {
            try
            {              
                    var item = database.Find<Verification>(i => i.Username == username);
                    if (item == null)
                        return -1;
                    if (password != item.Password)
                        return -1;
                    return item.PersonId;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error checking credentials!! {e.Message}");
                return 0;
            }
        }

        public Verification GetVerification(int personId)
        {
            return database.Get<Verification>(i => i.PersonId == personId);
        }
    }
}
