using System;
using System.Web.Security;
using JumpStart;

namespace ExampleMVC.Services
{
    public class AccountRoleService : IAccountRoleService
    {
        private readonly RoleProvider _provider;

        public AccountRoleService() : this(null) { }

        public AccountRoleService(RoleProvider provider)
        {
            _provider = provider ?? Roles.Provider;
        }

        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            Argument.IsNot.Null(usernames, "usernames");
            Argument.IsNot.Null(roleNames, "roleNames");

            _provider.AddUsersToRoles(usernames, roleNames);
        }

        public void CreateRole(string roleName)
        {
            Argument.IsNot.NullOrEmpty(roleName, "roleName");

            _provider.CreateRole(roleName);
        }

        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            Argument.IsNot.NullOrEmpty(roleName, "roleName");

            return _provider.DeleteRole(roleName, throwOnPopulatedRole);
        }

        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            Argument.IsNot.NullOrEmpty(roleName, "roleName");
            Argument.IsNot.NullOrEmpty(usernameToMatch, "usernameToMatch");

            return _provider.FindUsersInRole(roleName, usernameToMatch);
        }

        public string[] GetAllRoles()
        {
            return _provider.GetAllRoles();
        }

        public string[] GetRolesForUser(string username)
        {
            Argument.IsNot.NullOrEmpty(username, "username");

            return _provider.GetRolesForUser(username);
        }

        public string[] GetUsersInRole(string roleName)
        {
            Argument.IsNot.NullOrEmpty(roleName, "roleName");

            return _provider.GetUsersInRole(roleName);
        }

        public bool IsUserInRole(string username, string roleName)
        {
            Argument.IsNot.NullOrEmpty(username, "username");
            Argument.IsNot.NullOrEmpty(roleName, "roleName");

            return _provider.IsUserInRole(username, roleName);
        }

        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            Argument.IsNot.Null(usernames, "usernames");
            Argument.IsNot.Null(roleNames, "roleNames");

            _provider.RemoveUsersFromRoles(usernames, roleNames);
        }

        public bool RoleExists(string roleName)
        {
            Argument.IsNot.NullOrEmpty(roleName, "roleName");

            return _provider.RoleExists(roleName);
        }
    }
}
