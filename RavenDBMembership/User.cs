﻿/*
 * Original code from http://blogs.taiga.nl/martijn/2010/11/25/raven-db-asp-net-membership-provider/
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RavenDBMembership
{
	public class User
	{
		public string Id { get; set; }
		public string ApplicationName { get; set; }
		public string Username { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateLastLogin { get; set; }
		public IList<string> Roles { get; set; }

		public User()
		{
			Roles = new List<string>();
			Id = "raven/authorization/users/"; // db assigns id
		}
	}
}
