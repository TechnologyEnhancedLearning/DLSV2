namespace DigitalLearningSolutions.Api.Tests.Controllers.V1
{
    using System.Linq;
    using System.Reflection;
    using System.Security.Claims;
    using Asp.Versioning;
    using DigitalLearningSolutions.Api.Controllers.V1;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class SelfAssessmentControllerTests
    {
        private const int LearningHubAuthId = 1234;
        private const int DelegateUserId = 56;
        private const int SelfAssessmentId = 7;

        private SelfAssessmentController controller = null!;
        private ISelfAssessmentDataService selfAssessmentDataService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();
            userDataService = A.Fake<IUserDataService>();
            controller = new SelfAssessmentController(selfAssessmentDataService, userDataService);
            SetUser(new Claim(ClaimTypes.NameIdentifier, LearningHubAuthId.ToString()));
        }

        [Test]
        public void GetSelfAssessment_returns_ok_with_self_assessment_for_linked_user()
        {
            // Given
            var selfAssessment = new CurrentSelfAssessment { Id = SelfAssessmentId, Name = "Test assessment" };
            A.CallTo(() => userDataService.GetUserIdFromLearningHubAuthId(LearningHubAuthId)).Returns(DelegateUserId);
            A.CallTo(() => selfAssessmentDataService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(selfAssessment);

            // When
            var result = controller.GetSelfAssessment(SelfAssessmentId);

            // Then
            result.Result.Should().BeOkObjectResult().ValueAs<CurrentSelfAssessment>().Should().Be(selfAssessment);
        }

        [Test]
        public void GetSelfAssessment_returns_not_found_when_self_assessment_does_not_exist_for_user()
        {
            // Given
            A.CallTo(() => userDataService.GetUserIdFromLearningHubAuthId(LearningHubAuthId)).Returns(DelegateUserId);
            A.CallTo(() => selfAssessmentDataService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId))
                .Returns(null);

            // When
            var result = controller.GetSelfAssessment(SelfAssessmentId);

            // Then
            result.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void GetSelfAssessment_returns_forbidden_when_learning_hub_user_is_not_linked_to_dls()
        {
            // Given
            A.CallTo(() => userDataService.GetUserIdFromLearningHubAuthId(LearningHubAuthId)).Returns(null);

            // When
            var result = controller.GetSelfAssessment(SelfAssessmentId);

            // Then
            result.Result.Should().BeOfType<ForbidResult>();
            A.CallTo(() => selfAssessmentDataService.GetSelfAssessmentForCandidateById(A<int>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void GetSelfAssessment_returns_unauthorized_when_token_has_no_learning_hub_auth_id()
        {
            // Given
            SetUser(new Claim(ClaimTypes.Email, "someone@example.com"));

            // When
            var result = controller.GetSelfAssessment(SelfAssessmentId);

            // Then
            result.Result.Should().BeOfType<UnauthorizedResult>();
            A.CallTo(() => userDataService.GetUserIdFromLearningHubAuthId(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => selfAssessmentDataService.GetSelfAssessmentForCandidateById(A<int>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void SelfAssessmentController_requires_authorization()
        {
            // Then
            typeof(SelfAssessmentController).GetCustomAttribute<AuthorizeAttribute>().Should().NotBeNull();
            typeof(SelfAssessmentController).GetMethod(nameof(SelfAssessmentController.GetSelfAssessment))!
                .GetCustomAttribute<AllowAnonymousAttribute>().Should().BeNull();
        }

        [Test]
        public void SelfAssessmentController_is_version_1()
        {
            // When
            var versions = typeof(SelfAssessmentController)
                .GetCustomAttributes<ApiVersionAttribute>()
                .SelectMany(a => a.Versions);

            // Then
            versions.Should().BeEquivalentTo(new[] { new ApiVersion(1, 0) });
        }

        private void SetUser(params Claim[] claims)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer")),
                },
            };
        }
    }
}
