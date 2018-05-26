using Prism.Mvvm;
using Prism.Navigation;
using WorkHours.Models;
using WorkHours.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WorkHours.ViewModels
{
    public class SearchPageViewModel:BindableBase, INavigatedAware
    {
        private readonly INavigationService _navigationService;
        private readonly IPersonService _personService;

        public SearchPageViewModel(IPersonService personService, INavigationService navigationService)
        {
            _navigationService = navigationService;
            _personService = personService;
        }

        /// <summary>
        /// entry text property
        /// </summary>
        string searchTerm = string.Empty;
        public string SearchTerm
        {
            get => searchTerm;
            set => SetProperty(ref searchTerm, value, ()=> {  SearchEmployees(); });
        }

        /// <summary>
        /// List items property
        /// </summary>
        ObservableCollection<Person> employees = new ObservableCollection<Person>();
        public ObservableCollection<Person> Employees
        {
            get => employees;
            set => SetProperty(ref employees, value);
        }

        /// <summary>
        /// Selected employee from the list,
        /// go to the admin page to see his details and send him as navigation parameter
        /// </summary>
        Person selectedEmployee = null;
        public Person SelectedEmployee
        {
            get => selectedEmployee;
            set => SetProperty(ref selectedEmployee, value, async () =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("employee", SelectedEmployee);
                await _navigationService.NavigateAsync("AdminPage", parameters);
            });
        }

        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {

        }
        public void OnNavigatedFrom(INavigationParameters navigationParameters)
        {

        }

        /// <summary>
        /// Clear the current list of presented employees,
        /// get the list of employees by the search term inserted and add them to the list
        /// </summary>
        void SearchEmployees()
        {
            Employees.Clear();
            var listEmployees = _personService.GetEmployees(SearchTerm);
            foreach (var elem in listEmployees)
                Employees.Add(elem);
        }
    }
}
