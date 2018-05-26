using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkHours.Models
{

    public class CheckInOut
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Date { get; set; } 
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int TimeElapsed { get; set; }
        [NotNull]
        public int PersonId { get; set; }
    }
}
