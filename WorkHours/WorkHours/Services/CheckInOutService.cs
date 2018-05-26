using WorkHours.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace WorkHours.Services
{
    public class CheckInOutService:ICheckInOutService
    {
        private SQLite.SQLiteConnection database;
        public CheckInOutService()
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

        public CheckInOut GetActiveCheckInOut(string date, int personId)
        {
            try
            {
                return database.Get<CheckInOut>(i => i.PersonId == personId && i.EndTime == null);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"error getting checkinout object: {e.Message}");
            }
            return null;
        }

        public bool SaveCheckInOut(CheckInOut checkInOut)
        {
            try
            {
                if (checkInOut.TimeElapsed < 0)
                    checkInOut.TimeElapsed += 24 * 60;
                return database.Insert(checkInOut) > 0;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error saving checkinout {e.Message}");
            }
            return false;
        }

        public bool UpdateCheckInOut(CheckInOut checkInOut)
        {

            try
            {
                if (checkInOut.TimeElapsed < 0)
                    checkInOut.TimeElapsed += 24 * 60;
                return database.Update(checkInOut) > 0;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error saving checkinout {e.Message}");
            }
            return false;
        }
        public List<CheckInOut> GetMonthCheckInOut(int month, int personId)
        {
            try
            {
                var query = database.Table<CheckInOut>().Where(i => i.PersonId == personId);
                List<CheckInOut> retList = new List<CheckInOut>();
                foreach (var item in query)
                    if (IsMonth(month, item.Date))
                        retList.Add(item);
                return retList;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error: {e.Message}");
                return null;
            }
        }
        public bool IsMonth(int month, string date)
        {
            try
            {
                var Date = Convert.ToDateTime(date);
                if (Date.Month == month)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error {e.Message}");
                return false;
            }
        }
        public List<CheckInOut> GetDayCheckInOut(string date, int personId)
        {
            try
            {
                var query = database.Table<CheckInOut>()?.Where(i => i.PersonId == personId)?.ToList();
                List<CheckInOut> list = new List<CheckInOut>();
                foreach (var item in query)
                    if (IsDate(item.Date, date))
                        list.Add(item);
                return list;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error: {e.Message}");
                return null;
            }
        }
        bool IsDate(string date, string currDate)
        {
            var Date = Convert.ToDateTime(date);
            var currentDate = Convert.ToDateTime(currDate);
            if (Date.Day == currentDate.Day && Date.Month == currentDate.Month && Date.Year == currentDate.Year)
                return true;
            return false;
        }
        public bool DeleteCheckInOut()
        {
            try
            {
                var list = database.Table<CheckInOut>().Where(i => i.TimeElapsed < 0).ToList();
                foreach (var elem in list)
                    database.Delete(elem);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error: {e.Message}");
                return false;
            }
            return true;
        }
    }
}
