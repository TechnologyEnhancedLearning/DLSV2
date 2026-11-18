namespace DigitalLearningSolutions.Web.API
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// API endpoints for self assessments.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/self-assessments")]
    public class SelfAssessmentsController : ControllerBase
    {
        private readonly ISelfAssessmentService selfAssessmentService;

        public SelfAssessmentsController(ISelfAssessmentService selfAssessmentService)
        {
            this.selfAssessmentService = selfAssessmentService;
        }

        /// <summary>
        /// Gets a self assessment for a delegate.
        /// </summary>
        /// <remarks>
        /// This is a secured API and a valid bearer token with correct read scope is required
        /// as an authentication header while making this request.
        /// This GET request requires the SelfAssessmentID and the DelegateUserID and it gets the
        /// basic details of the delegate and associated self assessment. 
        /// </remarks>
        /// <param name="selfAssessmentId">The self assessment identifier.</param>
        /// <param name="delegateUserId">The delegate user identifier.</param>
        /// <returns>The requested self assessment as JSON.</returns>
        [HttpGet("{selfAssessmentId:int}")]
        [ProducesResponseType(typeof(CurrentSelfAssessment), 200)]
        [ProducesResponseType(404)]
        public ActionResult<CurrentSelfAssessment> GetSelfAssessment(
            int selfAssessmentId,
            [FromQuery] int delegateUserId
        )
        {
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                delegateUserId,
                selfAssessmentId
            );

            return selfAssessment == null ? NotFound() : Ok(selfAssessment);
        }
    }
}
