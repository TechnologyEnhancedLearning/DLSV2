namespace DigitalLearningSolutions.Api.Controllers.V1
{
    using Asp.Versioning;
    using DigitalLearningSolutions.Api.Authentication;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Self assessment endpoints (version 1).
    /// </summary>
    /// <remarks>
    /// All actions need a valid Learning Hub bearer token in the Authorization header.
    /// The caller is identified by the Learning Hub auth ID in the token, which is mapped to a DLS user.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class SelfAssessmentController : ControllerBase
    {
        private readonly ISelfAssessmentDataService selfAssessmentDataService;
        private readonly IUserDataService userDataService;

        public SelfAssessmentController(
            ISelfAssessmentDataService selfAssessmentDataService,
            IUserDataService userDataService
        )
        {
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.userDataService = userDataService;
        }

        /// <summary>
        /// Gets a self assessment for a delegate.
        /// </summary>
        /// <remarks>
        /// This is a secured API and a valid bearer token is required
        /// as an authentication header while making this request.
        /// This GET request requires the SelfAssessmentID as an input and it gets the
        /// basic details of the delegate and associated self assessment.
        /// </remarks>
        /// <param name="selfAssessmentId">The self assessment identifier.</param>
        /// <returns>The requested self assessment as JSON.</returns>
        /// <response code="200">The self assessment for the signed in delegate.</response>
        /// <response code="401">The bearer token is missing, invalid or expired, or has no Learning Hub auth ID.</response>
        /// <response code="403">The Learning Hub account is not linked to a DLS user.</response>
        /// <response code="404">The delegate is not enrolled on a self assessment with this ID.</response>
        [HttpGet("{selfAssessmentId:int}")]
        [ProducesResponseType(typeof(CurrentSelfAssessment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CurrentSelfAssessment> GetSelfAssessment(int selfAssessmentId)
        {
            // Get the learningHubAuthId from the claims in the validated bearer token
            // (the same claim the Web project reads in Startup.OnTicketReceived).
            var learningHubAuthId = User.GetLearningHubAuthId();
            if (learningHubAuthId == null)
            {
                return Unauthorized();
            }

            // Map the Learning Hub account to the DLS user, as the Web project does at sign in.
            var candidateUserId = userDataService.GetUserIdFromLearningHubAuthId(learningHubAuthId.Value);
            if (candidateUserId == null)
            {
                // A valid Learning Hub user who has no linked DLS account may not use the DLS API.
                return Forbid();
            }

            var selfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(
                candidateUserId.Value,
                selfAssessmentId
            );

            return selfAssessment == null ? NotFound() : Ok(selfAssessment);
        }
    }
}
