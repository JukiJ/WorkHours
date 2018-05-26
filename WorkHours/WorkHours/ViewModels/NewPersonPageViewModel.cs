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
    public class NewPersonPageViewModel : BindableBase, INavigatedAware
    {
        private readonly IPersonService _personService;
        private readonly ICredentialsService _credentialsService;
        public NewPersonPageViewModel(IPersonService personService, ICredentialsService credentialsService)
        {
            _personService = personService;
            _credentialsService = credentialsService;
        }

        public Person CurrentUser { get; set; }
        public Person Employee { get; set; }

        public Verification Verification { get; set; }

        public bool IsEdit = false;

        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            CurrentUser = App.Current.Properties["currentUser"] as Person;

            IsEdit = navigationParameters.GetValue<bool>("edit");

            Employee = navigationParameters.GetValue<Person>("employee");
            if (Employee != null) //If employee is sent as parameter then that means that person is beign updated
            {
                Name = Employee.Name;
                Surname = Employee.Surname;
                Position = Employee.Position;
                IsAdmin = Employee.Admin;
                IsEnabled = Employee.Admin;
                Verification = _credentialsService.GetVerification(Employee.Id);
                Username = Verification.Username;
                Password = Verification.Password;
            }
            else if (IsEdit) //If employee is not sent and IsEdit is set to true that means that CurrentUser is updating his profile
            {
                Name = CurrentUser.Name;
                Surname = CurrentUser.Surname;
                Position = CurrentUser.Position;
                IsAdmin = CurrentUser.Admin;
                IsEnabled = CurrentUser.Admin;
                Verification = _credentialsService.GetVerification(CurrentUser.Id);
                Username = Verification.Username;
                Password = Verification.Password;
            }
            else //if IsEdit is set to false, CurrentUser is creating a new user
                Employee = new Person() { Name = "Name", Surname = "Surname", Admin = false, Position = "Undefined" };
        }
        public void OnNavigatedFrom(INavigationParameters navigationParameters)
        {

        }

        /// <summary>
        /// Users name
        /// </summary>
        private string name=string.Empty;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        /// <summary>
        /// Users surname
        /// </summary>
        private string surname = string.Empty;
        public string Surname
        {
            get => surname;
            set => SetProperty(ref surname, value);
        }

        /// <summary>
        /// users username
        /// </summary>
        string username = string.Empty;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        /// <summary>
        /// Users password
        /// </summary>
        string password = string.Empty;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }
        /// <summary>
        /// Checkbox, is user admin
        /// </summary>
        bool isAdmin = false;
        public bool IsAdmin
        {
            get => isAdmin;
            set
            {
                SetProperty(ref isAdmin, value);
                if (isAdmin)
                    ButtonColor = "Blue";
                else
                    ButtonColor = "White";
            }
        }

        /// <summary>
        /// Checkbox IsEnabled property,
        /// true when the user is admin
        /// </summary>
        bool isEnabled = false;
        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }

        /// <summary>
        /// Users position on a job
        /// </summary>
        string position = string.Empty;
        public string Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }
        /// <summary>
        /// Checkbox button background color,
        /// set to blue if it is set
        /// </summary>
        string buttonColor = "White";
        public string ButtonColor
        {
            get => buttonColor;
            set => SetProperty(ref buttonColor, value);
        }

        Command setAdminCommand;
        public Command SetAdminCommand => setAdminCommand ?? (setAdminCommand = new Command(() => ExecuteSetAdmin()));

        void ExecuteSetAdmin()
        {
            IsAdmin=!IsAdmin;
        }

        Command saveCommand;
        public Command SaveCommand => saveCommand ?? (saveCommand = new Command(async () => await ExecuteSavePerson()));
        async Task ExecuteSavePerson()
        {
            if (!IsEdit) //If IsEdit is set to false then user is creating a new person
            {
                await CreateNewPerson();
            }
            else if(Employee !=null) // If IsEdit is set to true and employee is not null then update the employee
            {
                await UpdateEmployee();
            }
            else //If IsEdit is true and Employee is null then update the current user profile
            {
                await UpdateCurrentUser();
            }
        }
        async Task CreateNewPerson()
        {
            Employee.Name = Name;
            Employee.Surname = Surname;
            Employee.Position = Position;
            Employee.Admin = IsAdmin;
            Verification = new Verification() { Username = Username, Password = Password };
            if (_personService.CreateNewPerson(Employee, Verification))
                await App.Current.MainPage.DisplayAlert("Success", "You have created a new person succesfully!", "Great");
            else
                await App.Current.MainPage.DisplayAlert("Error", "Oh no, something went wrong!", "Ok");
        }
        async Task UpdateEmployee()
        {
            Employee.Name = Name;
            Employee.Surname = Surname;
            Employee.Position = Position;
            Employee.Admin = IsAdmin;
            Verification.Username = Username;
            Verification.Password = Password;
            if (_personService.UpdatePerson(Employee, Verification))
                await App.Current.MainPage.DisplayAlert("Success", "You have updated person's profile succesfully!", "Great");
            else
                await App.Current.MainPage.DisplayAlert("Error", "Oh no, something went wrong!", "Ok");
        }
        async Task UpdateCurrentUser()
        {
            CurrentUser.Name = Name;
            CurrentUser.Surname = Surname;
            CurrentUser.Position = Position;
            CurrentUser.Admin = IsAdmin;
            Verification.Username = Username;
            Verification.Password = Password;
            if (_personService.UpdatePerson(CurrentUser, Verification))
                await App.Current.MainPage.DisplayAlert("Success", "You have updated your profile succesfully!", "Great");
            else
                await App.Current.MainPage.DisplayAlert("Error", "Oh no, something went wrong!", "Ok");
        }
    }
}
