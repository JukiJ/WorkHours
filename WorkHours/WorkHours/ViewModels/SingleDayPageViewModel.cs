using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkHours.Helpers;
using Xamarin.Forms;
using WorkHours.Models;
using WorkHours.Services;

namespace WorkHours.ViewModels
{
    public class SingleDayPageViewModel:BindableBase, INavigatedAware
    {
        private readonly ICheckInOutService _checkInOutService;
        private readonly INavigationService _navigationService;
        public SingleDayPageViewModel(ICheckInOutService checkInOutService, INavigationService navigationService) 
        {
            _navigationService = navigationService;
            _checkInOutService = checkInOutService;
            CurrentDate = DateTime.Now.Date.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern);
        }
        public Person Employee { get; set; }
        public Person CurrentUser { get; set; }
        public ListViewItemTemplateModel Model { get; set; } //Day selected from previous page
        public bool IsAdmin = false;
        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            if(Employee==null)
                Employee = navigationParameters.GetValue<Person>("employee");
            CurrentUser = App.Current.Properties["currentUser"] as Person;
            if(Model==null)
                Model = navigationParameters.GetValue<ListViewItemTemplateModel>("SelectedItem");          
            
            var changedItem = navigationParameters.GetValue<CheckInOut>("object");
            if (changedItem != null) //If changedItem is not null then update the list
                UpdateList(changedItem);

            if (Model != null && changedItem==null)//If model is not null and changedItem is then user just selected the day from the list so just show it
            {
                TotalTime = Model.HoursWorked;
                GetAllCheckInOuts();
            }
        }
        public void OnNavigatedFrom(INavigationParameters navigationParameters)
        {
            navigationParameters.Add("object", Model);
        }

        #region Properties
        string currentDate = string.Empty;
        public string CurrentDate
        {
            get => currentDate;
            set => SetProperty(ref currentDate, value);
        }
        /// <summary>
        /// Items source for a list view
        /// </summary>
        ObservableCollection<CheckInOut> items = new ObservableCollection<CheckInOut>();
        public ObservableCollection<CheckInOut> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        /// <summary>
        /// Selected item property from list view
        /// </summary>
        CheckInOut selectedItem = null;
        public CheckInOut SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value, async ()=>
            {
                if (SelectedItem != null && IsAdmin)
                {
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add("object", SelectedItem);
                    await _navigationService.NavigateAsync("EditPopup", parameters);
                }
            }
            );
        }

        /// <summary>
        /// Total time worked that day
        /// </summary>
        string totalTime = string.Empty;
        public string TotalTime
        {
            get => totalTime;
            set => SetProperty(ref totalTime, value);
        }
#endregion

        /// <summary>
        /// Get all CHeckInOut's and show them
        /// </summary>
        void GetAllCheckInOuts()
        {
            List<CheckInOut> list = new List<CheckInOut>();

            if (Employee != null) // If employee is not null search his checkInOut times otherwise currentUser one's
            {
                IsAdmin = Employee.Admin;
                list = _checkInOutService.GetDayCheckInOut(Model.Date, Employee.Id);
            }
            else
            {
                IsAdmin = CurrentUser.Admin;
                list = _checkInOutService.GetDayCheckInOut(Model.Date, CurrentUser.Id);
            }

            if (list.Count>0) //If there aree CheckInOuts for that day show them
            foreach (var elem in list)
                Items.Add(elem);
        }

        /// <summary>
        /// If the user has changed one CheckInOut object update it in the database and reload the list
        /// </summary>
        /// <param name="checkInOut"></param>
        void UpdateList(CheckInOut checkInOut)
        {
            _checkInOutService.UpdateCheckInOut(checkInOut);
            Items.Clear();
            GetAllCheckInOuts();

            UpdateModel();
        }

        /// <summary>
        /// Model which represents one day and is sent back later to the DaysPage,
        /// Updae the HoursWorked of that model and TotalTime presented on this page
        /// </summary>
        void UpdateModel()
        {
            int mins = 0;
            foreach(var item in Items)
            {
                mins += item.TimeElapsed;
            }
            if(mins>0)
                Model.HoursWorked = (mins / 60).ToString();
            TotalTime = Model.HoursWorked;
        }
    }
}
