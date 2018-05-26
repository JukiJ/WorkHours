using WorkHours.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkHours.Services
{
    public interface ICheckInOutService
    {
        CheckInOut GetActiveCheckInOut(string date, int personId);
        bool SaveCheckInOut(CheckInOut checkInOut);
        bool UpdateCheckInOut(CheckInOut checkInOut);
        List<CheckInOut> GetMonthCheckInOut(int month, int personId);
        List<CheckInOut> GetDayCheckInOut(string date, int personId);
        bool DeleteCheckInOut();
    }
}
