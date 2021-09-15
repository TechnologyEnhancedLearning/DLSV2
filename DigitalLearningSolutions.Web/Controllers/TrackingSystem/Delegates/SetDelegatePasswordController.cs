namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.SetDelegatePassword;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/SetDelegatePassword/{delegateId:int}")]
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
        public IActionResult Index(int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserById(delegateId);

            if (delegateUser == null || delegateUser.CentreId != User.GetCentreId())
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(delegateUser.EmailAddress))
            {
                return View("NoEmail");
            }

            var referer = HttpContext.Request.GetTypedHeaders().Referer;
            var isFromViewDelegatePage = referer?.AbsolutePath.StartsWith("/TrackingSystem/Delegates/View") == true;
            
            var model = new SetDelegatePasswordViewModel(delegateUser.FullName, delegateId, isFromViewDelegatePage);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(SetDelegatePasswordViewModel model, int delegateId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var delegateUser = userDataService.GetDelegateUserById(delegateId);
            await passwordService.ChangePasswordAsync(delegateUser!.EmailAddress!, model.Password!);

            return View("Confirmation");
        }
    }
}
