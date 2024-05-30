﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.SetDelegatePassword;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/SetPassword")]
    public class SetDelegatePasswordController : Controller
    {
        private readonly IPasswordService passwordService;
        private readonly IUserService userService;

        public SetDelegatePasswordController(IPasswordService passwordService, IUserService userService)
        {
            this.passwordService = passwordService;
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Index(
            int delegateId,
            bool isFromViewDelegatePage,
            ReturnPageQuery? returnPageQuery = null
        )
        {
            var delegateUser = userService.GetDelegateUserById(delegateId)!;

            var model = new SetDelegatePasswordViewModel(
                DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(delegateUser.FirstName, delegateUser.LastName),
                delegateId,
                delegateUser.RegistrationConfirmationHash,
                isFromViewDelegatePage,
                returnPageQuery
            );

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(
            SetDelegatePasswordViewModel model,
            int delegateId,
            bool isFromViewDelegatePage
        )
        {
            if (!ModelState.IsValid)
            {
                model.IsFromViewDelegatePage = isFromViewDelegatePage;
                return View(model);
            }

            var delegateAccount = userService.GetDelegateAccountById(delegateId)!;

            await passwordService.ChangePasswordAsync(delegateAccount.UserId, model.Password!);

            return View("Confirmation");
        }
    }
}
