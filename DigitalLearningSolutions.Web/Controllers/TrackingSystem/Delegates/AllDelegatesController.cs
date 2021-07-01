namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/All")]
    public class AllDelegatesController : Controller
    {
        private readonly IUserDataService userDataService;

        public AllDelegatesController(IUserDataService userDataService)
        {
            this.userDataService = userDataService;
        }

        public IActionResult Index()
        {
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(User.GetCentreId()).GetRange(0, 10);
            var model = new AllDelegatesViewModel(delegateUsers);

            return View(model);
        }
    }
}
