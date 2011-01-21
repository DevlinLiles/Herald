/*
 * Original code from http://blogs.taiga.nl/martijn/2010/11/25/raven-db-asp-net-membership-provider/
 * Modified from XUnit to MSTest by Tim Rayburn (http://TimRayburn.net)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RavenDBMembership.Provider;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RavenDBMembership.Tests
{
    [TestClass]
	public class RoleTests : InMemoryStoreTestcase
	{
		[TestMethod]
		public void StoreRole()
		{
			var newRole = new Role("Users", null);

			using (var store = NewInMemoryStore())
			{
				using (var session = store.OpenSession())
				{
					session.Store(newRole);
					session.SaveChanges();
				}

				Thread.Sleep(500);

				using (var session = store.OpenSession())
				{
					var role = session.Query<Role>().FirstOrDefault();
					Assert.IsNotNull(role);
					Assert.AreEqual("raven/authorization/roles/Users", role.Id);
				}
			}
		}

        [TestMethod]
		public void StoreRoleWithApplicationName()
		{
			var newRole = new Role("Users", null);
			newRole.ApplicationName = "MyApplication";

			using (var store = NewInMemoryStore())
			{
				using (var session = store.OpenSession())
				{
					session.Store(newRole);
					session.SaveChanges();
				}

				Thread.Sleep(500);

				using (var session = store.OpenSession())
				{
					var role = session.Query<Role>().FirstOrDefault();
					Assert.IsNotNull(role);
					Assert.AreEqual("raven/authorization/roles/MyApplication/Users", role.Id);
				}
			}
		}

        [TestMethod]
		public void StoreRoleWithParentRole()
		{
			var parentRole = new Role("Users", null);
			var childRole = new Role("Contributors", parentRole);

			using (var store = NewInMemoryStore())
			{
				using (var session = store.OpenSession())
				{
					session.Store(parentRole);
					session.Store(childRole);
					session.SaveChanges();
				}

				Thread.Sleep(500);

				using (var session = store.OpenSession())
				{
					var roles = session.Query<Role>().ToList();
					Assert.AreEqual(2, roles.Count);
					var childRoleFromDb = roles.Single(r => r.ParentRoleId != null);
					Assert.AreEqual("raven/authorization/roles/Users/Contributors", childRoleFromDb.Id);
				}
			}
		}

        [TestMethod]
		public void RoleExists()
		{
			var appName = "APPNAME";
			var newRole = new Role("TheRole", null);
			newRole.ApplicationName = appName;

			using (var store = NewInMemoryStore())
			{
				using (var session = store.OpenSession())
				{
					session.Store(newRole);
					session.SaveChanges();
				}

				Thread.Sleep(500);

				var provider = new RavenDBRoleProvider();
				provider.DocumentStore = store;
				provider.ApplicationName = appName;
				Assert.IsTrue(provider.RoleExists("TheRole"));
			}
		}

        [TestMethod]
		public void AddUsersToRoles()
		{
			var roles = new Role[] { new Role("Role 1", null), new Role("Role 2", null), new Role("Role 3", null) };
			var user = new User();
			user.Username = "UserWithRole1AndRole2";

			using (var store = NewInMemoryStore())
			{
				using (var session = store.OpenSession())
				{
					foreach (var role in roles)
					{
						session.Store(role);
					}
					session.Store(user);
					session.SaveChanges();
				}

				Thread.Sleep(500);

				var provider = new RavenDBRoleProvider();
				provider.DocumentStore = store;
				provider.AddUsersToRoles(new [] { user.Username }, new [] { "Role 1", "Role 2" });

				Assert.IsTrue(provider.IsUserInRole(user.Username, "Role 1"));
				Assert.IsFalse(provider.IsUserInRole(user.Username, "Role 3"));
			}
		}

        [TestMethod]
		public void RemoveUsersFromRoles()
		{
			var roles = new Role[] { new Role("Role 1", null), new Role("Role 2", null), new Role("Role 3", null) };
			var user = new User();
			user.Username = "UserWithRole1AndRole2";

			using (var store = NewInMemoryStore())
			{
				using (var session = store.OpenSession())
				{
					foreach (var role in roles)
					{
						session.Store(role);
					}
					session.Store(user);
					session.SaveChanges();
				}

				Thread.Sleep(500);

				var provider = new RavenDBRoleProvider();
				provider.DocumentStore = store;
				provider.AddUsersToRoles(new [] { user.Username }, new [] { "Role 1", "Role 2" });

				Assert.IsTrue(provider.IsUserInRole(user.Username, "Role 1"));
				Assert.IsTrue(provider.IsUserInRole(user.Username, "Role 2"));

				provider.RemoveUsersFromRoles(new[] { user.Username }, new[] { "Role 1" });

				Assert.IsFalse(provider.IsUserInRole(user.Username, "Role 1"));
				Assert.IsTrue(provider.IsUserInRole(user.Username, "Role 2"));
			}
		}
	}
}
