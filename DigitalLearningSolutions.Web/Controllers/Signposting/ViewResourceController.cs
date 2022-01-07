using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.Controllers.Signposting
{
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.UseSignposting)]
    public class ViewResourceController : Controller
    {
        public ViewResourceController(IUserService userService, ILearningHubAuthClient learningHubAuthClient)
        {
            this.userService = userService;
            this.learningHubAuthClient = learningHubAuthClient;
        }

        private readonly IUserService userService;
        private readonly ILearningHubAuthClient learningHubAuthClient;

        [Route("Signposting/ViewResource/{resourceReferenceId}")]
        public IActionResult Index()
        {
            // check linking here

            int? learningHubAuthId = userService.GetDelegateUserLearningHubAuthId;

            if (learningHubAuthId.HasValue)
            {
                // go to sso/login
                learningHubAuthClient.LoginUserToLearningHub(learningHubAuthId.Value);
            }
            
            // go to sso/create-user
            learningHubAuthClient.CreateLearningHubUser();
        }
    }
}
