namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using NUnit.Framework;

    public class VerifyAdminUserCanAccessGroupCourseTests
    {
        private const int GroupCustomisationId = 25;
        private const int GroupId = 103;
        private ActionExecutingContext context = null!;
        private IGroupsService groupsService = null!;

        [SetUp]
        public void SetUp()
        {
            groupsService = A.Fake<IGroupsService>();

            var delegateGroupsController = new DelegateGroupsController(
                A.Fake<ICentreCustomPromptsService>(),
                A.Fake<IGroupsService>(),
                A.Fake<IUserService>(),
                A.Fake<ICourseService>(),
                A.Fake<IJobGroupsDataService>()
            ).WithDefaultContext().WithMockUser(true);
            context = ContextHelper.GetDefaultActionExecutingContext(delegateGroupsController);
            context.RouteData.Values["groupId"] = GroupId;
            context.RouteData.Values["groupCustomisationId"] = GroupCustomisationId;
        }

        [Test]
        public void Returns_NotFound_if_groupCourse_not_in_users_centre()
        {
            // Given
            A.CallTo(() => groupsService.GetUsableGroupCourseForCentre(A<int>._, A<int>._, A<int>._)).Returns(null);

            // When
            new VerifyAdminUserCanAccessGroupCourse(groupsService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Does_not_return_action_if_groupCourse_is_in_users_centre()
        {
            // Given
            A.CallTo(() => groupsService.GetUsableGroupCourseForCentre(A<int>._, A<int>._, A<int>._))
                .Returns(new GroupCourse());

            // When
            new VerifyAdminUserCanAccessGroupCourse(groupsService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
