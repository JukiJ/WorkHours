using Prism.Mvvm;
using Prism.Navigation;
using WorkHours.Models;
using WorkHours.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WorkHours.ViewModels
{
    public class AdminPageViewModel : BindableBase, INavigatedAware
    {
        private readonly ICountHours _countHours;
        private readonly INavigationService _navigationService;
        private readonly IPersonService _personService;
        private readonly ICheckInOutService _checkInOutService;
        public AdminPageViewModel(ICountHours countHours, INavigationService navigationService, IPersonService personService, ICheckInOutService checkInOutService)
        {
            _countHours = countHours;
            _navigationService = navigationService;
            _personService = personService;
            _checkInOutService = checkInOutService;
        }

        public Person Employee { get; set; } //Currently viewed employee

        /// <summary>
        /// Button Text property
        /// </summary>
        string employeeName=string.Empty;
        public string EmployeeName
        {
            get => employeeName;
            set => SetProperty(ref employeeName, value);
        }

        /// <summary>
        /// Label text property
        /// </summary>
        string employeePosition = string.Empty;
        public string EmployeePosition
        {
            get => employeePosition;
            set => SetProperty(ref employeePosition, value);
        }

        /// <summary>
        /// Hours worked this month, label text property
        /// </summary>
        string monthHours = string.Empty;
        public string MonthHours
        {
            get => monthHours;
            set => SetProperty(ref monthHours, value);
        }

        /// <summary>
        /// Hours worked this week, label text property
        /// </summary>
        string weekHours = string.Empty;
        public string WeekHours
        {
            get => weekHours;
            set => SetProperty(ref weekHours, value);
        }

        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            if(Employee == null)
                Employee = navigationParameters["employee"] as Person;
#if __ANDROID__
            var (month,week)=_countHours.GetHours(Employee.Id);
            MonthHours = month.ToString() + "/ 176";
            WeekHours = week.ToString() + "/ 40";
#endif

#if WINDOWS_UWP
            var list = _checkInOutService.GetMonthCheckInOut(DateTime.Now.Month, Employee.Id);
            var monthHours = _countHours.GetHoursThisMonth(list);
            var weekHours = _countHours.GetHoursThisWeek(list);

            MonthHours = monthHours.ToString() + "/ 176";
            WeekHours = weekHours.ToString() + "/ 40";
#endif

            EmployeeName = Employee.ToString();
            EmployeePosition = Employee.Position;
        }
        public void OnNavigatedFrom(INavigationParameters navigationParameters)
        {

        }

        /// <summary>
        /// Proceed to edit currently viewed employee
        /// </summary>
        Command editEmployeeCommand;
        public Command EditEmployeeCommand => editEmployeeCommand ?? (editEmployeeCommand = new Command(async () => await ExecuteEditEmployeeCommand()));
        async Task ExecuteEditEmployeeCommand()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("employee", Employee);
            parameters.Add("edit", true);
            await _navigationService.NavigateAsync("NewPersonPage", parameters);
        }

        /// <summary>
        /// Command to advance to list of the working hours per day page;
        /// Parameter type is sent to new page which says if the user wants to see monthly or weekly view
        /// </summary>
        Command checkHoursCommand;
        public Command CheckHoursCommand => checkHoursCommand ?? (checkHoursCommand = new Command<string>(async (string type) => await ExecuteCheckHoursCommand(type)));
        async Task ExecuteCheckHoursCommand(string type)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("type", type);
            parameters.Add("employee", Employee);
            await _navigationService.NavigateAsync("DaysPage", parameters);
        }

    }
}
