using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using WorkHours.Models;
using System.Linq;
using SQLite;

namespace WorkHours.Services
{
    class PersonService:IPersonService
    {
        private SQLiteConnection database;

        private static object collisionLock = new object();

        /// <summary>
        /// Instantiate the database and get connection
        /// </summary>
        /// <returns></returns>
        public PersonService()
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
        
        public Person GetPerson(int id)
        {
            lock (collisionLock)
            {
                return database.Get<Person>(i => i.Id == id);
            }
        }
        public bool CreateNewPerson(Person person, Verification verification)
        {
            try
            {
                lock (collisionLock)
                {
                    database.Insert(person);
                    verification.PersonId = person.Id;
                    database.Insert(verification);
                }
                return true;
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error: {e.Message}");
                return false;
            }
        }
        public bool UpdatePerson(Person person, Verification verification)
        {
            try
            {
                lock (collisionLock)
                {
                    database.Update(person);
                    database.Update(verification);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error: {e.Message}");
                return false;
            }
        }
        public IEnumerable<Person> GetEmployees(string searchTerm)
        {
            return database.Table<Person>().Where(i => i.Surname.ToLower().Contains(searchTerm.ToLower())).ToList();
        }
    }
}
