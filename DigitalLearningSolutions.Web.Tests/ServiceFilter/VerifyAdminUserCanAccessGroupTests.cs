namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using NUnit.Framework;

    public class VerifyAdminUserCanAccessGroupTests
    {
        private readonly int GroupId = 1;
        private readonly int UserCentreId = 101;
        private IGroupsService groupsService = null!;

        [SetUp]
        public void SetUp()
        {
            groupsService = A.Fake<IGroupsService>();
        }

        [Test]
        public void Returns_NotFound_if_centreIds_dont_match()
        {
            // Given
            var context = GetDefaultContext();
            A.CallTo(() => groupsService.GetGroupCentreId(A<int>._)).Returns(UserCentreId + 1);

            // When
            new VerifyAdminUserCanAccessGroup(groupsService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Does_not_return_action_if_centreIds_match()
        {
            // Given
            var context = GetDefaultContext();
            A.CallTo(() => groupsService.GetGroupCentreId(A<int>._)).Returns(UserCentreId);

            // When
            new VerifyAdminUserCanAccessGroup(groupsService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        private ActionExecutingContext GetDefaultContext()
        {
            var delegateGroupsController = new DelegateGroupsController(
                A.Fake<ICentreCustomPromptsService>(),
                A.Fake<IGroupsService>(),
                A.Fake<IUserService>(),
                A.Fake<ICourseService>(),
                A.Fake<IJobGroupsDataService>()
            ).WithDefaultContext().WithMockUser(true, UserCentreId);
            var context = ContextHelper.GetDefaultActionExecutingContext(delegateGroupsController);
            context.RouteData.Values["groupId"] = GroupId;

            return context;
        }
    }
}
