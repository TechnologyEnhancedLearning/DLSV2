namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Users
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
    using System;
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
            var model = new SetSuperAdminUserPasswordViewModel(dlsSubApplication);
            if (TempData["SearchString"] != null)
            {
                model.SearchString = Convert.ToString(TempData["SearchString"]);
            }
            if (TempData["FilterString"] != null)
            {
                model.ExistingFilterString = Convert.ToString(TempData["FilterString"]);
            }
            if (TempData["Page"] != null)
            {
                model.Page = Convert.ToInt16(TempData["Page"]);
            }
            model.UserId = userId;
            model.UserName = userEntity.UserAccount.FirstName + " " + userEntity.UserAccount.LastName + " (" + userEntity.UserAccount.PrimaryEmail + ")";
            return View("SuperAdminUserSetPassword", model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SetSuperAdminUserPasswordFormData formData, DlsSubApplication dlsSubApplication)
        {
            TempData.Keep("UserID");
            var userId = TempData["UserID"];

            if (!ModelState.IsValid)
            {
                var model = new SetSuperAdminUserPasswordViewModel(formData, dlsSubApplication);
                model.UserName = formData.UserName;
                return View("SuperAdminUserSetPassword", model);
            }

            var newPassword = formData.Password!;

            await passwordService.ChangePasswordAsync((int)userId, newPassword);

            //TODO: This feature will work after TD-995 is merged.This comment should be removed after the merge.
            TempData["UserId"] = userId;
            return RedirectToAction("Index", "Users", new { UserId = userId });
        }
    }
}
