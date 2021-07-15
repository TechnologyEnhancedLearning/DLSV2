namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/View/{delegateId:int}")]
    public class ViewDelegateController : Controller
    {
        private readonly CustomPromptHelper customPromptHelper;
        private readonly IUserDataService userDataService;

        public ViewDelegateController(IUserDataService userDataService, CustomPromptHelper customPromptHelper)
        {
            this.userDataService = userDataService;
            this.customPromptHelper = customPromptHelper;
        }

        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreId();
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);

            if (delegateUser == null || delegateUser.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            var customFields = customPromptHelper.GetCustomFieldViewModelsForCentre(centreId, delegateUser);
            var delegateInfo = new DelegateInfoViewModel(delegateUser, customFields);
            var model = new ViewDelegateViewModel(delegateInfo);

            return View(model);
        }
    }
}
