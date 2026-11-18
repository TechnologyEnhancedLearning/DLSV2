namespace DigitalLearningSolutions.Web.Tests.API
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.API;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class SelfAssessmentsControllerTests
    {
        private const int DelegateUserId = 11;
        private const int SelfAssessmentId = 1;

        private ISelfAssessmentService selfAssessmentService = null!;
        private SelfAssessmentsController controller = null!;

        [SetUp]
        public void SetUp()
        {
            selfAssessmentService = A.Fake<ISelfAssessmentService>();
            controller = new SelfAssessmentsController(selfAssessmentService);
        }

        [Test]
        public void GetSelfAssessment_returns_the_self_assessment_from_the_service()
        {
            var selfAssessment = new CurrentSelfAssessment { Id = SelfAssessmentId, Name = "POC assessment" };
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            var result = controller.GetSelfAssessment(SelfAssessmentId, DelegateUserId);

            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeSameAs(selfAssessment);
        }

        [Test]
        public void GetSelfAssessment_returns_not_found_when_the_service_returns_no_self_assessment()
        {
            A.CallTo(() => selfAssessmentService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(null);

            var result = controller.GetSelfAssessment(SelfAssessmentId, DelegateUserId);

            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
