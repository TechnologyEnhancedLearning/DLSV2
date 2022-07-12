namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class VerifyDelegateUserCanAccessSelfAssessmentTests
    {
        private const int DelegateId = 2;
        private const int SelfAssessmentId = 1;
        private ILogger<VerifyDelegateUserCanAccessSelfAssessment> logger = null!;
        private ISelfAssessmentService selfAssessmentService = null!;

        [SetUp]
        public void Setup()
        {
            selfAssessmentService = A.Fake<ISelfAssessmentService>();
            logger = A.Fake<ILogger<VerifyDelegateUserCanAccessSelfAssessment>>();
        }

        [Test]
        public void Returns_Redirect_to_access_denied_if_delegate_does_not_have_self_assessment()
        {
            // Given
            var context = GetDefaultContext();
            A.CallTo(() => selfAssessmentService.CanDelegateAccessSelfAssessment(A<int>._, A<int>._)).Returns(false);

            // When
            new VerifyDelegateUserCanAccessSelfAssessment(selfAssessmentService, logger).OnActionExecuting(context);

            // Then
            using (new AssertionScope())
            {
                context.Result.Should().BeRedirectToActionResult().WithActionName("AccessDenied")
                    .WithControllerName("LearningSolutions");
            }
        }

        [Test]
        public void Does_not_return_access_denied_if_delegate_has_self_assessment()
        {
            // Given
            var context = GetDefaultContext();
            A.CallTo(() => selfAssessmentService.CanDelegateAccessSelfAssessment(A<int>._, A<int>._)).Returns(true);

            // When
            new VerifyDelegateUserCanAccessSelfAssessment(selfAssessmentService, logger).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        private ActionExecutingContext GetDefaultContext()
        {
            var delegateGroupsController = new RecommendedLearningController(
                A.Fake<IFilteredApiHelperService>(),
                A.Fake<ISelfAssessmentService>(),
                A.Fake<IConfiguration>(),
                A.Fake<IRecommendedLearningService>(),
                A.Fake<IActionPlanService>(),
                A.Fake<ISearchSortFilterPaginateService>()
            ).WithDefaultContext().WithMockUser(true, delegateId: DelegateId);
            var context = ContextHelper.GetDefaultActionExecutingContext(delegateGroupsController);
            context.RouteData.Values["selfAssessmentId"] = SelfAssessmentId;

            return context;
        }
    }
}
