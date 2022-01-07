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
        public IActionResult Index(int resourceReferenceId)
        {
            // check linking here
            var delegateId = User.GetCandidateIdKnownNotNull();

            int? learningHubAuthId = userService.GetDelegateUserLearningHubAuthId(delegateId);

            if (learningHubAuthId.HasValue)
            {
                // get resource link from DB

                // redirect here to sso/login endpoint with auth id, hash, etc (see document)
            }

            // redirect to create-user endpoint

            // go to sso/create-user
            //learningHubAuthClient.CreateLearningHubUser();
        }
    }
}
