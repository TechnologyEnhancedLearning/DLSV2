namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/All")]
    public class AllDelegatesController : Controller
    {
        private readonly CustomPromptHelper customPromptHelper;
        private readonly IUserDataService userDataService;

        public AllDelegatesController(IUserDataService userDataService, CustomPromptHelper customPromptHelper)
        {
            this.userDataService = userDataService;
            this.customPromptHelper = customPromptHelper;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId).Take(10);
            var searchableDelegateViewModels = delegateUsers.Select(
                delegateUser =>
                {
                    var customFields = customPromptHelper.GetCustomFieldViewModelsForCentre(centreId, delegateUser);
                    return new SearchableDelegateViewModel(delegateUser, customFields);
                }
            );
            var model = new AllDelegatesViewModel(centreId, searchableDelegateViewModels);

            return View(model);
        }
    }
}
