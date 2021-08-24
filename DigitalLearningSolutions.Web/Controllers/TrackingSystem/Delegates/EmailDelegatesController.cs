namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/Email")]
    public class EmailDelegatesController : Controller
    {
        private readonly IUserService userService;

        public EmailDelegatesController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var delegateUsers = userService.GetDelegateUserCardsForWelcomeEmail(centreId);
            var model = new EmailDelegatesViewModel(delegateUsers);

            return View(model);
        }
    }
}
