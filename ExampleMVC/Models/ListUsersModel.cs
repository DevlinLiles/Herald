using System;
using System.Collections.Generic;
using System.Web.Security;

namespace ExampleMVC.Models
{
    public class ListUsersModel
    {
        public IList<MembershipUser> Users { get; set; }
    }
}
