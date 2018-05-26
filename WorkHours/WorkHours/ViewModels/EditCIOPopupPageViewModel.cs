using Prism.Mvvm;
using Prism.Navigation;
using WorkHours.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace WorkHours.ViewModels
{
    public class EditCIOPopupPageViewModel:BindableBase, INavigatedAware
    {
        private readonly INavigationService _navigationService;
        public EditCIOPopupPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        public CheckInOut CheckInOut { get; set; } //CheckInOut object beign updated
        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            CheckInOut = navigationParameters.GetValue<CheckInOut>("object");
            if(CheckInOut != null)
            {
                From = CheckInOut.StartTime;
                To = CheckInOut.EndTime;
            }
        }
        public void OnNavigatedFrom(INavigationParameters navigationParameters)
        {

        }

        /// <summary>
        /// Start time from selected CheckInOut object
        /// </summary>
        string from = string.Empty;
        public string From
        {
            get=> from;
            set => SetProperty(ref from, value);
        }

        /// <summary>
        /// End time from selected CheckInOut object
        /// </summary>
        string to = string.Empty;
        public string To
        {
            get => to;
            set => SetProperty(ref to, value);
        }


        /// <summary>
        /// Command to save the changes made to the CheckInOutObject and send it as a parameter to a last page
        /// </summary>
        Command saveChangesCommand;
        public Command SaveChangesCommand => saveChangesCommand ?? (saveChangesCommand = new Command(async () =>
        {
            CheckInOut.StartTime = From;
            CheckInOut.EndTime = To;
            TimeSpan span = Convert.ToDateTime(CheckInOut.EndTime) - Convert.ToDateTime(CheckInOut.StartTime);            
            CheckInOut.TimeElapsed = (int)span.TotalMinutes;
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("object", CheckInOut);
            await _navigationService.GoBackAsync(parameters);            
        }));

        /// <summary>
        /// Command to discard changes made to this CheckInOut object
        /// </summary>
        Command discardChangesCommand;
        public Command DiscardChangesCommand => discardChangesCommand ?? (discardChangesCommand = new Command(async () => await _navigationService.GoBackAsync()));
    }
}
