using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.Controllers.Signposting
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.UseSignposting)]
    public class ViewResourceController : Controller
    {
        public ViewResourceController(IUserService userService, ILearningResourceReferenceDataService learningResourceReferenceDataService, ILearningHubSsoSecurityService learningHubSsoSecurityService, IConfiguration config)
        {
            this.userService = userService;
            this.learningResourceReferenceDataService = learningResourceReferenceDataService;
            this.learningHubSsoSecurityService = learningHubSsoSecurityService;
            this.config = config;
        }

        private readonly IUserService userService;
        private readonly ILearningResourceReferenceDataService learningResourceReferenceDataService;
        private readonly ILearningHubSsoSecurityService learningHubSsoSecurityService;
        private readonly IConfiguration config;

        private const string LoginEndpointRelativePath = "/login";
        private const string CreateUserEndpointRelativePath = "/create-user";

        [Route("Signposting/ViewResource/{resourceReferenceId}")]
        public IActionResult Index(int resourceReferenceId)
        {
            // check linking here
            var delegateId = User.GetCandidateIdKnownNotNull();

            int? learningHubAuthId = userService.GetDelegateUserLearningHubAuthId(delegateId);

            var authEndpoint = config.GetLearningHubAuthApiBaseUrl();

            var clientCode = config.GetLearningHubAuthApiClientCode();

            if (learningHubAuthId.HasValue)
            {
                var resourceUrl = learningResourceReferenceDataService.GetLearningHubResourceReferenceById(resourceReferenceId); // TODO url encoding?

                var idHash = learningHubSsoSecurityService.GenerateHash(learningHubAuthId.ToString());

                var loginQueryString =
                    $"?clientcode={clientCode}&userid={learningHubAuthId}&hash={idHash}&endclientUrl={resourceUrl}";

                return Redirect(authEndpoint + LoginEndpointRelativePath + loginQueryString);
            }

            var state = Guid.NewGuid().ToString(); // TODO is this correct?

            var stateHash = learningHubSsoSecurityService.GenerateHash(state);

            var createUserQueryString = $"?clientcode={clientCode}&state={state}&hash={stateHash}";

            return Redirect(authEndpoint + CreateUserEndpointRelativePath+ createUserQueryString);
        }
    }
}
