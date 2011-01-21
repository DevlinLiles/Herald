/*
 * Original code from http://blogs.taiga.nl/martijn/2010/11/25/raven-db-asp-net-membership-provider/
 * Modified from XUnit to MSTest by Tim Rayburn (http://TimRayburn.net)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Client;
using RavenDBMembership.Provider;
using System.Web.Security;
using Raven.Client;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RavenDBMembership.Tests
{
    [TestClass]
	public class UserTests : InMemoryStoreTestcase
	{
		[TestMethod]
		public void RunRavenInMemory()
		{
			using (var store = NewInMemoryStore())
			{
				Assert.IsNotNull(store);
			}
		}

        [TestMethod]
		public void StoreUserShouldCreateId()
		{
			var newUser = new User { Username = "martijn", FullName = "Martijn Boland" };
			var newUserIdPrefix = newUser.Id;

			using (var store = NewInMemoryStore())
			{
				using (var session = store.OpenSession())
				{
					session.Store(newUser);
					session.SaveChanges();
				}
			}

			Assert.AreEqual(newUserIdPrefix + "1", newUser.Id);
		}

        [TestMethod]
		public void CreateNewMembershipUserShouldCreateUserDocument()
		{
			using (var store = NewInMemoryStore())
			{
				var provider = new RavenDBMembershipProvider();
				provider.DocumentStore = store;
				MembershipCreateStatus status;
				var membershipUser = provider.CreateUser("martijn", "1234ABCD", "martijn@boland.org", null, null, true, null, out status);
				Assert.AreEqual(MembershipCreateStatus.Success, status);
				Assert.IsNotNull(membershipUser);
				Assert.IsNotNull(membershipUser.ProviderUserKey);
				Assert.AreEqual("martijn", membershipUser.UserName);
			}
		}

        [TestMethod]
		public void ChangePassword()
		{
			using (var store = NewInMemoryStore())
			{
				// Arrange
				var provider = new RavenDBMembershipProvider();
				provider.DocumentStore = store;
				MembershipCreateStatus status;
				var membershipUser = provider.CreateUser("martijn", "1234ABCD", "martijn@boland.org", null, null, true, null, out status);

				// Act
				provider.ChangePassword("martijn", "1234ABCD", "DCBA4321");
				Thread.Sleep(500);

				// Assert
				Assert.IsTrue(provider.ValidateUser("martijn", "DCBA4321"));
			}
		}

        [TestMethod]
		public void DeleteUser()
		{
			using (var store = NewInMemoryStore())
			{
				// Arrange
				var provider = new RavenDBMembershipProvider();
				provider.DocumentStore = store;
				MembershipCreateStatus status;
				var membershipUser = provider.CreateUser("martijn", "1234ABCD", "martijn@boland.org", null, null, true, null, out status);

				// Act
				provider.DeleteUser("martijn", true);

				// Assert
				Thread.Sleep(500);
				using (var session = store.OpenSession())
				{
					Assert.AreEqual(0, session.Query<User>().Count());
				}
			}
		}

        [TestMethod]
		public void GetAllUsersShouldReturnAllUsers()
		{
			using (var store = NewInMemoryStore())
			{
				// Arrange
				CreateUsersInDocumentStore(store, 5);
				var provider = new RavenDBMembershipProvider();
				provider.DocumentStore = store;
				
				// Act
				Thread.Sleep(500);
				int totalRecords;
				var membershipUsers = provider.GetAllUsers(0, 10, out totalRecords);

				// Assert
				Assert.AreEqual(5, totalRecords);				
				Assert.AreEqual(5, membershipUsers.Count);				
			}
		}

        [TestMethod]
		public void FindUsersByUsernamePart()
		{
			using (var store = NewInMemoryStore())
			{
				// Arrange
				CreateUsersInDocumentStore(store, 5);
				var provider = new RavenDBMembershipProvider();
				provider.DocumentStore = store;

				// Act
				Thread.Sleep(500);
				int totalRecords;
				var membershipUsers = provider.FindUsersByName("ser", 0, 10, out totalRecords); // Usernames are User1 .. Usern

				// Assert
				Assert.AreEqual(5, totalRecords); // All users should be returned
				Assert.AreEqual(5, membershipUsers.Count);
			}
		}

        [TestMethod]
		public void FindUsersWithPaging()
		{
			using (var store = NewInMemoryStore())
			{
				// Arrange
				CreateUsersInDocumentStore(store, 10);
				var provider = new RavenDBMembershipProvider();
				provider.DocumentStore = store;

				// Act
				Thread.Sleep(500);
				int totalRecords;
				var membershipUsers = provider.GetAllUsers(0, 5, out totalRecords);

				// Assert
				Assert.AreEqual(10, totalRecords); // All users should be returned
				Assert.AreEqual(5, membershipUsers.Count);
			}
		}

        [TestMethod]
		public void FindUsersForDomain()
		{
			using (var store = NewInMemoryStore())
			{
				// Arrange
				CreateUsersInDocumentStore(store, 10);
				var provider = new RavenDBMembershipProvider();
				provider.DocumentStore = store;

				// Act
				Thread.Sleep(500);
				int totalRecords;
				var membershipUsers = provider.FindUsersByEmail("@foo.bar", 0, 2, out totalRecords);
				int totalRecordsForUnknownDomain;
				var membershipUsersForUnknownDomain = provider.FindUsersByEmail("@foo.baz", 0, 2, out totalRecordsForUnknownDomain);

				// Assert
				Assert.AreEqual(10, totalRecords); // All users should be returned
				Assert.AreEqual(2, membershipUsers.Count);
				Assert.AreEqual(0, totalRecordsForUnknownDomain);
				Assert.AreEqual(0, membershipUsersForUnknownDomain.Count);
			}
		}

		private void CreateUsersInDocumentStore(IDocumentStore store, int numberOfUsers)
		{
			var users = CreateDummyUsers(numberOfUsers);
			using (var session = store.OpenSession())
			{
				foreach (var user in users)
				{
					session.Store(user);
				}
				session.SaveChanges();
			}
		}

		private IList<User> CreateDummyUsers(int numberOfUsers)
		{
			var users = new List<User>(numberOfUsers);
			for (int i = 0; i < numberOfUsers; i++)
			{
				users.Add(new User { Username = String.Format("User{0}", i), Email = String.Format("User{0}@foo.bar", i) });
			}
			return users;
		}
	}
}
