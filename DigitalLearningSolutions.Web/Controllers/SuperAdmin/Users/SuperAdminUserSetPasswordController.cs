﻿namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Users
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users;
    using DocumentFormat.OpenXml.Presentation;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System.Threading.Tasks;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Admins))]
    [Route("SuperAdmin/Users/SuperAdminUserSetPassword/{userId:int}")]
    public class SuperAdminUserSetPasswordController : Controller
    {
        private readonly IPasswordService passwordService;
        private readonly IUserService userService;
        public SuperAdminUserSetPasswordController(IPasswordService passwordService, IUserService userService)
        {
            this.passwordService = passwordService;
            this.userService = userService;
        }
        [HttpGet]
        public IActionResult Index(int userId, DlsSubApplication dlsSubApplication)
        {
            var userEntity = userService.GetUserById(userId);
            TempData["UserID"] = userId;
            TempData["UserName"] = userEntity.UserAccount.FirstName + " " + userEntity.UserAccount.LastName + " (" + userEntity.UserAccount.PrimaryEmail + ")";
            var model = new SetSuperAdminUserPasswordViewModel(dlsSubApplication);
            return View("SuperAdminUserSetPassword", model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SetSuperAdminUserPasswordFormData formData, DlsSubApplication dlsSubApplication)
        {
            TempData.Peek("UserID");
            var userId = TempData["UserID"];


            if (!ModelState.IsValid)
            {
                var model = new SetSuperAdminUserPasswordViewModel(formData, dlsSubApplication);
                return View("SuperAdminUserSetPassword", model);
            }

            var newPassword = formData.Password!;

            await passwordService.ChangePasswordAsync((int)userId, newPassword);

            //Reload user account page here.Waiting for TD-992 to completed
            //var model1 = new UserAccountsViewModel();
            //return View("Index", model1);

            //TODO: This feature will work after TD-995 is merged.This comment should be removed after the merge.
            TempData["UserId"] = userId;
            return RedirectToAction("Index", "Users", new { UserId = userId });
        }
    }
}