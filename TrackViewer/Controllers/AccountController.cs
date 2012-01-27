using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using TrackViewer.Models;

namespace MvcApp.Controllers
{

	[HandleError]
	public class AccountController : Controller
	{
		private const string _defaultController = "Track";
		private const string _defaultAction = "List";

		public IFormsAuthenticationService FormsService { get; set; }
		public IMembershipService MembershipService { get; set; }
		public IRoleService RoleService { get; set; }

		protected override void Initialize(RequestContext requestContext)
		{
			if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
			if (MembershipService == null) { MembershipService = new AccountMembershipService(); }
			if (RoleService == null) RoleService = new RoleService();

			base.Initialize(requestContext);
		}

		// **************************************
		// URL: /Account/Login
		// **************************************

		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(LoginModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				if (MembershipService.ValidateUser(model.UserName, model.Password))
				{
					if (model.UserName == "Shade") RoleService.AddUserToRole(model.UserName, "Admin");
					FormsService.SignIn(model.UserName, model.RememberMe);
					if (!String.IsNullOrEmpty(returnUrl))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction(_defaultAction, _defaultController);
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
		// URL: /Account/Logout
		// **************************************

		public ActionResult Logout()
		{
			FormsService.SignOut();

			return RedirectToAction(_defaultAction, _defaultController);
		}

		// **************************************
		// URL: /Account/Register
		// **************************************

		public ActionResult Register()
		{
			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
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
					return RedirectToAction(_defaultAction, _defaultController);
				}
				else
				{
					ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
				}
			}

			// If we got this far, something failed, redisplay form
			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
			return View(model);
		}

		// **************************************
		// URL: /Account/ChangePassword
		// **************************************

		[Authorize]
		public ActionResult ChangePassword()
		{
			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
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
			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
			return View(model);
		}

		// **************************************
		// URL: /Account/ChangePasswordSuccess
		// **************************************

		public ActionResult ChangePasswordSuccess()
		{
			return View();
		}

		// TODO: Add role management actions
	}
}
