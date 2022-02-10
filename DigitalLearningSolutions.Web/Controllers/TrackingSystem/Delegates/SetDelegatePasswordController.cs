namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Models.Enums;
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
        private readonly IUserDataService userDataService;

        public SetDelegatePasswordController(IPasswordService passwordService, IUserDataService userDataService)
        {
            this.passwordService = passwordService;
            this.userDataService = userDataService;
        }

        [HttpGet]
        public IActionResult Index(int delegateId, bool isFromViewDelegatePage, int? returnPage)
        {
            var delegateUser = userDataService.GetDelegateUserById(delegateId)!;

            if (string.IsNullOrWhiteSpace(delegateUser.EmailAddress))
            {
                return View("NoEmail");
            }

            var model = new SetDelegatePasswordViewModel(
                delegateUser.FullName,
                delegateId,
                returnPage,
                isFromViewDelegatePage
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

            var delegateUser = userDataService.GetDelegateUserById(delegateId)!;

            if (string.IsNullOrWhiteSpace(delegateUser.EmailAddress))
            {
                return View("NoEmail");
            }

            await passwordService.ChangePasswordAsync(delegateUser!.EmailAddress!, model.Password!);

            return View("Confirmation");
        }
    }
}
