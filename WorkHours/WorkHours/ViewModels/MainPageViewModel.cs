using Prism.Mvvm;
using Prism.Navigation;
using WorkHours.Models;
using WorkHours.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WorkHours.ViewModels
{
    public class MainPageViewModel:BindableBase, INavigatedAware
    {
        private INavigationService _navigationService;
        private readonly IPersonService _personService;
        private readonly ICheckInOutService _checkInOutService;
        private readonly ICountHours _countHours;
        public MainPageViewModel(INavigationService navigationService, IPersonService personService, ICountHours countHours, ICheckInOutService checkInOutService)
        {
            _checkInOutService = checkInOutService;
            _navigationService = navigationService;
            _personService = personService;
            _countHours = countHours;
        }
        public bool isLoggedIn; //If the user has started his work timer
        public Person CurrentUser { get; set; } 
        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            if(CurrentUser==null)
                CurrentUser=_personService.GetPerson(navigationParameters.GetValue<int>("PersonId")); 
            if(CurrentUser!=null)
            {
                App.Current.Properties.Remove("currentUser");
                App.Current.Properties.Add("currentUser", CurrentUser);
            }


            CurrentDate = DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern);
            CurrentTime = DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortTimePattern);

            var activeCheckInOut = _checkInOutService.GetActiveCheckInOut(DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern), CurrentUser.Id);
            if (activeCheckInOut != null)
            {
                ButtonColor = "Red";
                isLoggedIn = true;
            }

            Title = CurrentUser.ToString();
            IsAdmin = CurrentUser.Admin;

            SetStartStopButton();

            RefreshHours();

        }
        public void OnNavigatedFrom(INavigationParameters navigationParameters)
        {

        }
#region Binded properties
        /// <summary>
        /// Title text property
        /// </summary>
        string title = string.Empty;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        /// <summary>
        /// IsVisible property for toolbar items that can be used only by admins
        /// </summary>
        bool isAdmin = true;
        public bool IsAdmin
        {
            get => isAdmin;
            set => SetProperty(ref isAdmin, value);
        }

        /// <summary>
        /// Current date label text property
        /// </summary>
        string currentDate = string.Empty;
        public string CurrentDate
        {
            get => currentDate;
            set => SetProperty(ref currentDate, value);
        }

        /// <summary>
        /// Current time text property
        /// </summary>
        string currentTime = string.Empty;
        public string CurrentTime
        {
            get => currentTime;
            set => SetProperty(ref currentTime, value);
        }

        /// <summary>
        /// Start/Stop button text property
        /// </summary>
        string startStopButton = string.Empty;
        public string StartStopButton
        {
            get => startStopButton;
            set => SetProperty(ref startStopButton, value);
        }

        /// <summary>
        /// StartStop button background color property
        /// </summary>
        string buttonColor = "Green";
        public string ButtonColor
        {
            get => buttonColor;
            set => SetProperty(ref buttonColor, value);
        }

        /// <summary>
        /// Hours this week label text property
        /// </summary>
        string hoursThisWeek = string.Empty;
        public string HoursThisWeek
        {
            get => hoursThisWeek;
            set => SetProperty(ref hoursThisWeek, value);
        }

        /// <summary>
        /// Hours this month label text property
        /// </summary>
        string hoursThisMonth = string.Empty;
        public string HoursThisMonth
        {
            get => hoursThisMonth;
            set => SetProperty(ref hoursThisMonth, value);
        }
#endregion

        /// <summary>
        /// Set the text property on the start/stop button depending on if the user has already signed in
        /// </summary>
        void SetStartStopButton()
        {
            if (isLoggedIn)
                Device.BeginInvokeOnMainThread(()=>StartStopButton = "Kraj");
            else
                Device.BeginInvokeOnMainThread(()=>StartStopButton = "Početak");
        }

#if __ANDROID__
        void RefreshHours()
        {
            var (month, week) = _countHours.GetHours(CurrentUser.Id);

            HoursThisMonth = month.ToString() + "/ 176";
            HoursThisWeek = week.ToString() + "/ 40";
        }

#endif

#if WINDOWS_UWP
        void RefreshHours()
        {
            var list = _checkInOutService.GetMonthCheckInOut(DateTime.Now.Month, CurrentUser.Id);
            var monthHours = _countHours.GetHoursThisMonth(list);
            var weekHours = _countHours.GetHoursThisWeek(list);

            HoursThisMonth = monthHours.ToString() + "/ 176";
            HoursThisWeek = weekHours.ToString() + "/ 40";
        }
#endif

        /// <summary>
        /// Command to advance to list of the working hours per day page;
        /// Parameter type is sent to new page which says if the user wants to see monthly or weekly view
        /// </summary>
        Command checkHoursCommand;
        public Command CheckHoursCommand => checkHoursCommand ?? (checkHoursCommand = new Command<string>(async (string type) => await ExecuteCheckHoursCommand(type)));
        async Task ExecuteCheckHoursCommand(string type)
        {
            try
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("type", type);
                await _navigationService.NavigateAsync("DaysPage", parameters);
            }
            catch(Exception e)
            {
                Debug.WriteLine($"ERROR:                   {e.Message}");
            }
        }

        /// <summary>
        /// Execute the button command to start counting the time or stop,
        /// if the user is already logged in update the checkInOut with EndTime and calculate TimeElapsed,
        /// if not then create new CheckInOut object and set isLogged in to true
        /// </summary>
        Command startStopTimeCommand;
        public Command StartStopTimeCommand => startStopTimeCommand ?? (startStopTimeCommand = new Command(async () => await ExecuteStartStopTimeCommand()));
        async Task ExecuteStartStopTimeCommand()
        {
            if(isLoggedIn)
            {
                ButtonColor = "Green";
                var time = DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
                var activeCheckInOut= _checkInOutService.GetActiveCheckInOut(DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern), CurrentUser.Id);
                TimeSpan span = Convert.ToDateTime(time) - Convert.ToDateTime(activeCheckInOut.StartTime);
                activeCheckInOut.EndTime = time;
                activeCheckInOut.TimeElapsed = (int)span.TotalMinutes;
                if (activeCheckInOut.TimeElapsed < 0)
                    activeCheckInOut.TimeElapsed += (24 * 60);

                await UpdateCheckInOut(activeCheckInOut);                
            }
            else
            {
                ButtonColor = "Red";
                await SaveCheckInOut();
            }
        }

        async Task UpdateCheckInOut(CheckInOut checkInOut)
        {
            if (_checkInOutService.UpdateCheckInOut(checkInOut))
            {
                await App.Current.MainPage.DisplayAlert("Great", "You have succesfully saved your exit time", "Good!");
                isLoggedIn = false;
                SetStartStopButton();
            }
            else
                await App.Current.MainPage.DisplayAlert("OOOPS", "There seems to be a problem saving your exit time!", "Oh no!");
        }

        async Task SaveCheckInOut()
        {
            CheckInOut cio = new CheckInOut() { Date = DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern), StartTime = DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortTimePattern), PersonId = CurrentUser.Id };
            if (_checkInOutService.SaveCheckInOut(cio))
            {
                await App.Current.MainPage.DisplayAlert("Great", "You have succesfully saved your enter time", "Good!");
                isLoggedIn = true;
                SetStartStopButton();
            }
            else
                await App.Current.MainPage.DisplayAlert("OOOPS", "There seems to be a problem saving your enter time!", "Oh no!");
        }
        /// <summary>
        /// Go to the current user's profile page,
        /// send true as "edit" parameter
        /// </summary>
        Command profilePageCommand;
        public Command ProfilePageCommand => profilePageCommand ?? (profilePageCommand = new Command(async () => await ExecuteProfilePageCommand()));
        async Task ExecuteProfilePageCommand()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("edit", true);
            await _navigationService.NavigateAsync("NewPersonPage", parameters);
        }

        /// <summary>
        /// Command to advance to the search page, available only for admins
        /// </summary>
        Command searchPageCommand;
        public Command SearchPageCommand => searchPageCommand ?? (searchPageCommand = new Command(async () => await ExecuteSearchPageCommand()));
        async Task ExecuteSearchPageCommand()
        {
            await _navigationService.NavigateAsync("SearchPage");
        }

        /// <summary>
        /// Go to new person page,
        /// Since it is to create a new person send a false "edit" parameter,
        /// available only for admins
        /// </summary>
        Command newPersonPageCommand;
        public Command NewPersonPageCommand => newPersonPageCommand ?? (newPersonPageCommand = new Command(async () =>
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("edit", false);
            await _navigationService.NavigateAsync("NewPersonPage");
        }));

        /// <summary>
        /// As there is just a simple auth service no tokens or anything is issued so to logout just send the user back to the login screen
        /// </summary>
        Command logoutCommand;
        public Command LogoutCommand => logoutCommand ?? (logoutCommand = new Command(async () => await ExecuteLogoutCommand()));
        async Task ExecuteLogoutCommand()
        {
            App.Current.Properties.Remove("currentUser");
            await _navigationService.NavigateAsync("app:///LoginPage");
        }
    }
}
