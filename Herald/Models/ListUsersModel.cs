using System;
using System.Collections.Generic;
using System.Web.Security;

namespace Herald.Models
{
    public class ListUsersModel
    {
        public IList<MembershipUser> Users { get; set; }
    }
}
