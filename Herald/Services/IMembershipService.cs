using System;
using System.Web.Security;
using System.Collections.Generic;

namespace ExampleMVC.Services
{
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        MembershipUser GetUser(string userName, bool userIsOnline);
        bool UserExists(string userName);
        IList<MembershipUser> GetAllUsers();
    }
}
