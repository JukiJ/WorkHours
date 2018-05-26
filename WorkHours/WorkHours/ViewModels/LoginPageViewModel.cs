using Prism.Mvvm;
using Prism.Navigation;
using WorkHours.Models;
using WorkHours.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WorkHours.ViewModels
{
    public class LoginPageViewModel:BindableBase, INavigatedAware
    {
        private readonly INavigationService _navigationService;
        private readonly ICredentialsService _credentialsService;
        public LoginPageViewModel(INavigationService navigationService, ICredentialsService credentialsService)
        {
            _navigationService = navigationService;
            _credentialsService = credentialsService;
        }
        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {

        }
        public void OnNavigatedFrom(INavigationParameters navigationParameters)
        {

        }

        /// <summary>
        /// Text property from the username entry
        /// </summary>
        string username = string.Empty;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        /// <summary>
        /// Text property from the password entry
        /// </summary>
        string password = string.Empty;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        /// <summary>
        /// LoginCommand, Check entered credentials and recieve personId,
        /// if the personId is equal to -1 then the username and password are invalid,
        /// else advance to the "MainPage" and send "personId" as a parameter
        /// </summary>
        Command loginCommand;
        public Command LoginCommand => loginCommand ?? (loginCommand = new Command(async () => await ExecuteLoginCommand()));
        async Task ExecuteLoginCommand()
        {
            int personId = _credentialsService.CheckCredentials(Username, Password);
            if(personId==-1)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Invalid username or password!", "OK");
                return;
            }

            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("PersonId", personId);
            await _navigationService.NavigateAsync("/NavigationPage/MainPage", parameters);
        }
    }  
}
