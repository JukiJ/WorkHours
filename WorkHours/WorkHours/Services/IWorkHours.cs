using WorkHours.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkHours.Services
{
    public interface ICountHours
    {
#if __ANDROID__
        (int, int) GetHours(int id);
#endif
#if WINDOWS_UWP
        int GetHoursThisMonth(List<CheckInOut> list);
        int GetHoursThisWeek(List<CheckInOut> list);
#endif
    }
}
