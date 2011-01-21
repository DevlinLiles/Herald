using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ExampleMVC.Models;
using ExampleMVC.Services;
using ExampleMVC.Validation;
using JumpStart;

namespace ExampleMVC.Controllers
{
    public class AccountController : BaseController
    {
        private const string ROLE_Administrator = "Administrator";

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        public IAccountRoleService AccountRoleService { get; set; }

        public AccountController(IFormsAuthenticationService formsService, IMembershipService membershipService, IAccountRoleService accountRoleService)
        {
            this.FormsService = formsService;
            this.MembershipService = membershipService;
            this.AccountRoleService = accountRoleService;
        }

        // **************************************
        // URL: /Account
        // * Should display a list of users.
        // **************************************
        public ActionResult Index()
        {
            var model = new ListUsersModel()
                {
                    Users = MembershipService.GetAllUsers()
                };

            return View(model);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/CreateInitialAdmin
        // **************************************

        public ActionResult CreateInitialAdmin()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult CreateInitialAdmin(RegisterModel model)
        {
            MembershipCreateStatus status = MembershipCreateStatus.Success;

            if (ModelState.IsValid)
            {
                if (AccountRoleService.RoleExists(ROLE_Administrator) == false)
                    AccountRoleService.CreateRole(ROLE_Administrator);

                if (MembershipService.UserExists(model.UserName) == false)
                    status = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (status == MembershipCreateStatus.Success)
                {
                    AccountRoleService.AddUsersToRoles(new[] { model.UserName }, new[] { ROLE_Administrator });

                    FormsService.SignIn(model.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(status));
            }

            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }
}
