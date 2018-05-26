using WorkHours.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkHours.Services
{
    public class CountHours:ICountHours
    {
        private ICheckInOutService _checkInOutService;
        public CountHours(ICheckInOutService checkInOutService)
        {
            _checkInOutService = checkInOutService;
        }
#if __ANDROID__
        public (int, int) GetHours(int id)
        {
            var list = _checkInOutService.GetMonthCheckInOut(DateTime.Now.Month, id);
            if (list.Count > 0)
            {
                var month = GetHoursThisMonth(list);
                var week = GetHoursThisWeek(list);
                return (month, week);
            }
            return (0, 0);
        }
#endif

        /// <summary>
        /// Count the sum of all TimeElapsed fields from every CheckInOut element in a list and that is the amount of hours this person worked this month
        /// </summary>
        /// <param name="list"></param>
        public int GetHoursThisMonth(List<CheckInOut> list)
        {
            int h = 0;
            foreach (var item in list)
                h += item.TimeElapsed;
            return h/60;
        }

        /// <summary>
        /// Check what day of the week is and return the number of that day
        /// </summary>
        /// <param name="list"></param>
        int CheckDayOfWeek()
        {
            switch (DateTime.Now.DayOfWeek.ToString())
            {
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
                case "Sunday":
                    return 7;
                default:
                    return 0;
            }

        }

        /// <summary>
        /// Get the items from list of the current day and some days back depending on the number of the current day of the week,
        /// and count the sum of the hours worked each day
        /// </summary>
        /// <param name="list"></param>
        /// <param name="num"></param>
        public int GetHoursThisWeek(List<CheckInOut> list)
        {
            var numberOfDay = CheckDayOfWeek();
            List<CheckInOut> WeekList = new List<CheckInOut>();
            int w = 0;
            for (int i = 0; i < numberOfDay; i++)
            {
                var items = list.Where(t => t.Date == DateTime.Now.AddDays(-i).ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern)).ToList();
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        w += item.TimeElapsed;
                        WeekList.Add(item);
                    }
                }
            }
            return w/60;
        }
    }
}
