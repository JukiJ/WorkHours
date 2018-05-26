using WorkHours.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkHours.Services
{
    public interface ICredentialsService
    {
        int CheckCredentials(string username, string password);
        Verification GetVerification(int personId);
    }
}
