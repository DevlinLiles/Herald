using System;
using System.Web.Mvc;
using Herald.Core;
using Herald.Core.Domain;
using Herald.Core.Services;
using Herald.Core.ViewModels;
using Herald.Infrastructure.Repositories;
using Herald.UI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herald.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        private AccountController _accountController;
        private IRepository _repository;
        private MockFormsAuthenticationService _authenticationService;
        private MockMembershipService _membershipService;
        private MockCurrentUserService _currentUser;

        [TestInitialize]
        public void Before_each_test()
        {
            // Arrange

            _repository = new InMemoryRepository(true);
            _authenticationService = new MockFormsAuthenticationService();
            _membershipService = new MockMembershipService();
            _currentUser = new MockCurrentUserService();

            _accountController = new AccountController(_repository, _currentUser, _authenticationService, _membershipService);
        }

        [TestMethod]
        public void ChangePassword_Get_ReturnsView()
        {
            // Arrange
            _repository.Save(new UserProfile {Id = 1, Email = "good@email.com"});

            // Act
            ActionResult result = _accountController.ChangePassword(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void ChangePassword_Post_ReturnsRedirectOnSuccess()
        {
            // Arrange
            var model = new AccountChangePasswordModel
            {
                Email = "good@email.com",
                OldPassword = "goodOldPassword",
                NewPassword = "goodNewPassword",
                ConfirmPassword = "goodNewPassword"
            };

            // Act
            ActionResult result = _accountController.ChangePassword(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual("ChangePasswordSuccess", redirectResult.RouteValues["action"]);
        }

        [TestMethod]
        public void ChangePassword_Post_ReturnsViewIfChangePasswordFails()
        {
            // Arrange
            var model = new AccountChangePasswordModel
            {
                OldPassword = "goodOldPassword",
                NewPassword = "badNewPassword",
                ConfirmPassword = "badNewPassword"
            };

            // Act
            ActionResult result = _accountController.ChangePassword(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.ViewData.Model);
            Assert.AreEqual("The current password is incorrect or the new password is invalid.", _accountController.ModelState[""].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePassword_Post_ReturnsViewIfModelStateIsInvalid()
        {
            // Arrange
            var model = new AccountChangePasswordModel
            {
                OldPassword = "goodOldPassword",
                NewPassword = "goodNewPassword",
                ConfirmPassword = "goodNewPassword"
            };
            _accountController.ModelState.AddModelError("", "Dummy error message.");

            // Act
            ActionResult result = _accountController.ChangePassword(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.ViewData.Model);
        }

        [TestMethod]
        public void ChangePasswordSuccess_ReturnsView()
        {
            // Act
            ActionResult result = _accountController.ChangePasswordSuccess();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void LogOff_LogsOutAndRedirects()
        {
            // Act
            ActionResult result = _accountController.LogOff();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
            Assert.IsTrue(_authenticationService.SignOut_WasCalled);
        }

        [TestMethod]
        public void LogOn_Get_ReturnsView()
        {
            // Act
            ActionResult result = _accountController.LogOn("");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void LogOn_Post_ReturnsRedirectOnSuccess_WithoutReturnUrl()
        {
            // Arrange
            var model = new AccountLogOnModel
            {
                Email = "good@email.com",
                Password = "goodPassword",
                RememberMe = false
            };

            // Act
            ActionResult result = _accountController.LogOn(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
            Assert.IsTrue(_authenticationService.SignIn_WasCalled);
        }

        [TestMethod]
        public void LogOn_Post_ReturnsRedirectOnSuccess_WithReturnUrl()
        {
            // Arrange
            var model = new AccountLogOnModel
            {
                Email = "good@email.com",
                Password = "goodPassword",
                RememberMe = false,
                ReturnUrl = "/someUrl"
            };

            // Act
            ActionResult result = _accountController.LogOn(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirectResult = (RedirectResult)result;
            Assert.AreEqual("/someUrl", redirectResult.Url);
            Assert.IsTrue(_authenticationService.SignIn_WasCalled);
        }

        [TestMethod]
        public void LogOn_Post_ReturnsViewIfModelStateIsInvalid()
        {
            // Arrange
            var model = new AccountLogOnModel
            {
                Email = "good@email.com",
                Password = "goodPassword",
                RememberMe = false
            };
            _accountController.ModelState.AddModelError("", "Dummy error message.");

            // Act
            ActionResult result = _accountController.LogOn(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.ViewData.Model);
        }

        [TestMethod]
        public void LogOn_Post_ReturnsViewIfValidateUserFails()
        {
            // Arrange
            var model = new AccountLogOnModel
            {
                Email = "good@email.com",
                Password = "badPassword",
                RememberMe = false
            };

            // Act
            ActionResult result = _accountController.LogOn(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.ViewData.Model);
            Assert.AreEqual("The user name or password provided is incorrect.", _accountController.ModelState[""].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void Register_Get_ReturnsView()
        {
            // Act
            ActionResult result = _accountController.Register();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Register_Post_ReturnsRedirectOnSuccess()
        {
            // Arrange
            _repository.Save(new UserProfile {Id = 1, Email = "good@email.com"});
            var model = new AccountRegisterModel
            {
                Email = "good@email.com",
                Password = "goodPassword",
                ConfirmPassword = "goodPassword",
            };

            // Act
            ActionResult result = _accountController.Register(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual("Show", redirectResult.RouteValues["action"]);
        }

        [TestMethod]
        public void Register_Post_ReturnsViewIfRegistrationFails()
        {
            // Arrange
            var model = new AccountRegisterModel
            {
                Email = "duplicate@email.com",
                Password = "goodPassword",
                ConfirmPassword = "goodPassword"
            };

            // Act
            ActionResult result = _accountController.Register(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.ViewData.Model);
            Assert.AreEqual("Email already exists. Please enter a different email address.", _accountController.ModelState[""].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void Register_Post_ReturnsViewIfModelStateIsInvalid()
        {
            // Arrange
            var model = new AccountRegisterModel
            {
                Email = "good@email.com",
                Password = "goodPassword",
                ConfirmPassword = "goodPassword"
            };
            _accountController.ModelState.AddModelError("", "Dummy error message.");

            // Act
            ActionResult result = _accountController.Register(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(model, viewResult.ViewData.Model);
        }

        private class MockFormsAuthenticationService : IFormsAuthenticationService
        {
            public bool SignIn_WasCalled;
            public bool SignOut_WasCalled;

            public void SignIn(string email, bool createPersistentCookie)
            {
                // verify that the arguments are what we expected
                Assert.AreEqual("good@email.com", email);
                Assert.IsFalse(createPersistentCookie);

                SignIn_WasCalled = true;
            }

            public void SignOut()
            {
                SignOut_WasCalled = true;
            }
        }

        private class MockMembershipService : IMembershipService
        {
            public bool ValidateUser(string email, string password)
            {
                return (email == "good@email.com" && password == "goodPassword");
            }

            public string CreateUser(string email, string password)
            {
                if (email == "duplicate@email.com")
                    return "Email already exists. Please enter a different email address.";

                Assert.AreEqual("goodPassword", password);
                Assert.AreEqual("good@email.com", email);

                return "";
            }

            public bool ChangePassword(string email, string oldPassword, string newPassword)
            {
                return (email == "good@email.com" && oldPassword == "goodOldPassword" && newPassword == "goodNewPassword");
            }

            public void AddUserToRole(string email, string role)
            {
            }

            public void RemoveUserFromRole(string email, string role)
            {
            }

            public bool IsUserInRole(string email, string role)
            {
                return true;
            }

            public void Delete(string email)
            {
            }
        }

        private class MockCurrentUserService : ICurrentUserService
        {
            public string Email
            {
                get { return "good@email.com"; }
            }
            public bool IsSignedIn
            {
                get { return true; }
            }
            public bool IsAdmin
            {
                get { return false; }
            }

            public bool Is(string email)
            {
                return string.Equals(Email, email, StringComparison.OrdinalIgnoreCase);
            }

            public bool Is(UserProfile user)
            {
                return Is(user.Email);
            }

            public bool Owns(Presentation presentation)
            {
                return false;
            }

            public void AddRedirectMessage(string message)
            {
            }

            public string[] GetAndFlushRedirectMessages()
            {
                return new string[0];
            }
        }
    }
}