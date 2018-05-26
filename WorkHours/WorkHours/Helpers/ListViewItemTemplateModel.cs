using System;
using System.Collections.Generic;
using System.Text;

namespace WorkHours.Helpers
{
    public class ListViewItemTemplateModel
    {
        public int Id { get; set; }
        public string HoursWorked { get; set; }
        public string DayNumber { get; set; }
        public string Date { get; set; }
    }
}
