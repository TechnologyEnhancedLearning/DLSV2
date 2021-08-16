namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class DelegateGroupsControllerTests
    {
        private DelegateGroupsController delegateGroupsController = null!;
        private IGroupsDataService groupsDataService = null!;

        [SetUp]
        public void Setup()
        {
            groupsDataService = A.Fake<IGroupsDataService>();

            delegateGroupsController = new DelegateGroupsController(groupsDataService)
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void GroupDelegates_returns_not_found_with_incorrect_group_id_for_centre()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns(null);

            // When
            var result = delegateGroupsController.GroupDelegates(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void GroupDelegates_returns_view_result_with_correct_group_id_for_centre()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsDataService.GetGroupDelegates(1)).Returns(new List<GroupDelegate>());

            // When
            var result = delegateGroupsController.GroupDelegates(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            result.As<ViewResult>().Model.As<GroupDelegatesViewModel>().NavViewModel.GroupName.Should().Be("Group");
            result.As<ViewResult>().Model.As<GroupDelegatesViewModel>().NavViewModel.CurrentPage.Should()
                .Be(DelegateGroupPage.Delegates);
        }

        [Test]
        public void GroupCourses_returns_not_found_with_incorrect_group_id_for_centre()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns(null);

            // When
            var result = delegateGroupsController.GroupCourses(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void GroupCourses_returns_view_result_with_correct_group_id_for_centre()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsDataService.GetGroupCourses(1, 2)).Returns(new List<GroupCourse>());

            // When
            var result = delegateGroupsController.GroupCourses(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            result.As<ViewResult>().Model.As<GroupCoursesViewModel>().NavViewModel.GroupName.Should().Be("Group");
            result.As<ViewResult>().Model.As<GroupCoursesViewModel>().NavViewModel.CurrentPage.Should()
                .Be(DelegateGroupPage.Courses);
        }
    }
}
