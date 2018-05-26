using Prism.Mvvm;
using Prism.Navigation;
using WorkHours.Helpers;
using WorkHours.Models;
using WorkHours.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace WorkHours.ViewModels
{
    public class DaysPageViewModel:BindableBase, INavigatedAware
    {
        private readonly INavigationService _navigationService;
        private readonly ICheckInOutService _checkInOutService;
        public DaysPageViewModel(INavigationService navigationService, ICheckInOutService checkInOutService)
        {
            _navigationService = navigationService;
            _checkInOutService = checkInOutService;
        }
        public Person CurrentUser { get; set; }
        public Person Employee { get; set; } //Currently viewed employee
        public string Type { get; set; } //Is it month view or week view

        public int monthDifference = 0; //If the user views other months difference in number is counted here
        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            CurrentUser = App.Current.Properties["currentUser"] as Person;
            
            if(Employee==null)
                Employee = navigationParameters.GetValue<Person>("employee");

            var changedModel = navigationParameters.GetValue<ListViewItemTemplateModel>("object"); //If model for some day is changed update the list
            if (changedModel != null)
            {
                RefreshList();
            }
            else
            {
                if (string.IsNullOrEmpty(Type)) //if the type variable is currently null try to retrieve it from navigation parameters
                    Type = navigationParameters.GetValue<string>("type");

                List<CheckInOut> list = new List<CheckInOut>();//list of all CheckInOuts this month
                list = GetCheckInOuts();

                if (Type == "Month")
                {
                    ShowMonth(list, DateTime.Now.AddMonths(monthDifference).ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern));
                    IsMonth = true;
                }
                if (Type == "Week")
                    ShowWeek(list);
            }

        }
        public void OnNavigatedFrom(INavigationParameters navigationParameters)
        {

        }

        /// <summary>
        /// Currently viewed month, displayed on top of the screen
        /// </summary>
        string month = string.Empty;
        public string Month
        {
            get => month;
            set => SetProperty(ref month, value);
        }

        /// <summary>
        /// If the user is on a week view hide the buttons to switch month (false) , othervise show them (true)
        /// </summary>
        bool isMonth = false;
        public bool IsMonth
        {
            get => isMonth;
            set => SetProperty(ref isMonth, value);
        }

        #region ListView items
        /// <summary>
        /// Items source for a list view
        /// </summary>
        ObservableCollection<ListViewItemTemplateModel> items=new ObservableCollection<ListViewItemTemplateModel>();
        public ObservableCollection<ListViewItemTemplateModel> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        /// <summary>
        /// Selected item property from list view
        /// </summary>
        ListViewItemTemplateModel selectedItem = null;
        public ListViewItemTemplateModel SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value, async () =>
             {
                     NavigationParameters parameters = new NavigationParameters();
                     parameters.Add("SelectedItem", SelectedItem);

                     if (Employee != null) //If current user is currently watching another user send that user also as a parameter
                         parameters.Add("employee", Employee);

                     await _navigationService.NavigateAsync("SingleDay", parameters);
             });
        }
        #endregion

        #region Convert month from number to word
        void ConvertMonth()
        {
            switch (DateTime.Now.AddMonths(monthDifference).Month)
            {
                case 1:
                    Month = "January";
                    break;
                case 2:
                    Month = "February";
                    break;
                case 3:
                    Month = "March";
                    break;
                case 4:
                    Month = "April";
                    break;
                case 5:
                    Month = "May";
                    break;
                case 6:
                    Month = "June";
                    break;
                case 7:
                    Month = "July";
                    break;
                case 8:
                    Month = "August";
                    break;
                case 9:
                    Month = "September";
                    break;
                case 10:
                    Month = "October";
                    break;
                case 11:
                    Month = "November";
                    break;
                case 12:
                    Month = "December";
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Get all check in outs based on is the user watching his stats or one of the employees
        /// </summary>
        /// <returns></returns>
        List<CheckInOut> GetCheckInOuts()
        {
            if (Employee != null)
                return _checkInOutService.GetMonthCheckInOut(DateTime.Now.AddMonths(monthDifference).Month, Employee.Id);
            else
                return _checkInOutService.GetMonthCheckInOut(DateTime.Now.AddMonths(monthDifference).Month, CurrentUser.Id);
        }

        #region Month
        /// <summary>
        /// For each day of the current month shows the list of days with written number of hours worked on each day
        /// </summary>
        /// <param name="list"></param>
        void ShowMonth(List<CheckInOut> list, string date=null)
        {
            ConvertMonth();

            Items.Clear();

            int daysInMonth;

            DateTime DateSearched=DateTime.Now;

            if (date == null) //if the sent date is null set the daysInMonth based on the current month and year
                daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            else //else set the daysInMonth based on the searched date
            {
                DateSearched = Convert.ToDateTime(date);
                daysInMonth = DateTime.DaysInMonth(DateSearched.Year, DateSearched.Month);
            }
            for (int i=1;i<= daysInMonth; i++)
            {
                int id=i;
                // Create new ListViewItemTemplateModel for each day in month
                //If the date is null then use the current month and year, otherwise use the searched date
                ListViewItemTemplateModel item = new ListViewItemTemplateModel() { Id = id, DayNumber = i.ToString(), HoursWorked=CountWorkHours(list, i), Date= date==null ? i + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year : i+"/"+ DateSearched.Month+"/"+DateSearched.Year};
                Items.Add(item);
            }
        }
        /// <summary>
        /// Take all CheckInOut's from requested day and count time elapsed all together and return in as string,
        /// or "Nije radio" there are no CheckInOut's for that day,
        /// or "X" if that day is in future
        /// </summary>
        /// <param name="list"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        string CountWorkHours(List<CheckInOut> list, int day)
        {
            int totalMinutes = 0;
            foreach (var item in list.Where(i => Convert.ToDateTime(i.Date).Day == day)) //foreach item from list update the totalMinutes with TimeElapsed of that item
                totalMinutes += item.TimeElapsed;

            if (totalMinutes > 0)
                return (totalMinutes / 60).ToString();
            else if (day > DateTime.Now.Day && monthDifference == 0)
                return "X";
            else if (monthDifference > 0)
                return "X";
            return "Nije radio";
        }
        #endregion

        #region Week

        /// <summary>
        /// Count hours worked for a whole week up to and including the current day
        /// </summary>
        /// <param name="list"></param>
        /// <param name="numberOfDay"></param>
        void ShowWeek(List<CheckInOut> list)
        {
            int numberOfDay = GetDayNumber();
            Items.Clear(); //Clear the current list
            for (int i = numberOfDay - 1; i >= 0; i--) //Going reverse to have the right order from monday to sunday in list
            {
                int id = i;               
                ListViewItemTemplateModel item = new ListViewItemTemplateModel() { Id = id, DayNumber = DateTime.Now.AddDays(-i).DayOfWeek.ToString().Substring(0, 3), HoursWorked = CountWorkHours(list, DateTime.Now.Day - i), Date = id + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year };
                Items.Add(item);
            }
            SetOther(7 - numberOfDay); // (7-numberOfDay) gives the number of remaining days
        }

        /// <summary>
        /// Just return the number of the current day of the week
        /// </summary>
        /// <returns></returns>
        int GetDayNumber()
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
        /// Show the days until the end of the week, HoursWorked is default "X"
        /// </summary>
        /// <param name="num"></param>
        void SetOther(int num)
        {
            for(int i=1; i <= num; i++)
            {
                ListViewItemTemplateModel item = new ListViewItemTemplateModel() { Id = -1, DayNumber = DateTime.Now.AddDays(i).DayOfWeek.ToString().Substring(0,3), HoursWorked = "X" };
                Items.Add(item);
            }
        }
        #endregion

        /// <summary>
        /// Refreshes the whole list of days when the user makes a change in the list items or switches months
        /// </summary>
        void RefreshList()
        {
            var list = GetCheckInOuts();
            if (Type == "Month")
            {
                ShowMonth(list, DateTime.Now.AddMonths(monthDifference).ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern));
                IsMonth = true;
            }
            else if (Type == "Week")
                ShowWeek(list);
        }


        /// <summary>
        /// Command to switch to previous month,
        /// just decrement the monthDifference variable and Call the "ShowMonth" function with that month date
        /// </summary>
        Command previousMonthCommand;
        public Command PreviousMonthCommand => previousMonthCommand ?? (previousMonthCommand=new Command(() => ExecutePreviousMonthCommand()));
        void ExecutePreviousMonthCommand()
        {
            monthDifference--;
            List<CheckInOut> list = new List<CheckInOut>();
            list = GetCheckInOuts();
            ShowMonth(list, DateTime.Now.AddMonths(monthDifference).ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern));
        }

        /// <summary>
        /// Command to switch to next month,
        /// just increment the monthDifference variable and Call the "ShowMonth" function with that month date
        /// </summary>
        Command nextMonthCommand;
        public Command NextMonthCommand => nextMonthCommand ?? (nextMonthCommand = new Command(() => ExecuteNextMonthCommand()));
        void ExecuteNextMonthCommand()
        {
            monthDifference++;
            List<CheckInOut> list = new List<CheckInOut>();
            list = GetCheckInOuts();
            ShowMonth(list, DateTime.Now.AddMonths(monthDifference).ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern));
        }
    }
}
