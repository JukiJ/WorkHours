using WorkHours.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkHours.Services
{
    public interface IPersonService
    {
        Person GetPerson(int id);
        bool CreateNewPerson(Person person, Verification verification);
        bool UpdatePerson(Person person, Verification verification);
        IEnumerable<Person> GetEmployees(string searchTerm);
        

    }
}
