namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
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
        private IClockService clockService = null!;
        private DelegateGroupsController delegateGroupsController = null!;
        private IGroupsDataService groupsDataService = null!;

        [SetUp]
        public void Setup()
        {
            groupsDataService = A.Fake<IGroupsDataService>();
            clockService = A.Fake<IClockService>();

            delegateGroupsController = new DelegateGroupsController(groupsDataService, clockService)
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

        [Test]
        public void GroupDelegatesRemove_should_return_not_found_with_invalid_group_for_centre()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns(null);

            // When
            var result = delegateGroupsController.GroupDelegatesRemove(1, 2);

            // Them
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void GroupDelegatesRemove_should_return_not_found_with_delegate_not_in_group()
        {
            // Given
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsDataService.GetGroupDelegates(1)).Returns(new List<GroupDelegate>());

            // When
            var result = delegateGroupsController.GroupDelegatesRemove(1, 2);

            // Them
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void GroupDelegatesRemovePost_should_return_not_found_with_invalid_group_for_centre()
        {
            // Given
            var model = new GroupDelegatesRemoveViewModel { ConfirmRemovalFromGroup = true, RemoveProgress = true };
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns(null);

            // When
            var result = delegateGroupsController.GroupDelegatesRemove(model, 1, 2);

            // Them
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void GroupDelegatesRemovePost_should_return_not_found_with_delegate_not_in_group()
        {
            // Given
            var model = new GroupDelegatesRemoveViewModel { ConfirmRemovalFromGroup = true, RemoveProgress = true };
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsDataService.GetGroupDelegates(1)).Returns(new List<GroupDelegate>());

            // When
            var result = delegateGroupsController.GroupDelegatesRemove(model, 1, 2);

            // Them
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void GroupDelegatesRemove_should_return_view_if_unconfirmed()
        {
            // Given
            var model = new GroupDelegatesRemoveViewModel { ConfirmRemovalFromGroup = false };
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsDataService.GetGroupDelegates(1)).Returns(new List<GroupDelegate>{new GroupDelegate{DelegateId = 2}});

            // When
            var result = delegateGroupsController.GroupDelegatesRemove(model, 1, 2);

            // Then
            result.Should().BeViewResult();
            delegateGroupsController.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public void GroupDelegatesRemove_should_not_call_remove_progress_if_unchecked()
        {
            // Given
            var model = new GroupDelegatesRemoveViewModel { ConfirmRemovalFromGroup = true, RemoveProgress = false};
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsDataService.GetGroupDelegates(1)).Returns(new List<GroupDelegate> { new GroupDelegate { DelegateId = 2 } });
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(1, 2)).DoesNothing();

            // When
            var result = delegateGroupsController.GroupDelegatesRemove(model, 1, 2);

            // Then
            A.CallTo(() => groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(A<int>._, A<int>._, A<DateTime>._)).MustNotHaveHappened();
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(1, 2)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("GroupDelegates");
        }

        [Test]
        public void GroupDelegatesRemove_should_call_remove_progress_if_checked()
        {
            // Given
            var model = new GroupDelegatesRemoveViewModel { ConfirmRemovalFromGroup = true, RemoveProgress = true };
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsDataService.GetGroupDelegates(1)).Returns(new List<GroupDelegate> { new GroupDelegate { DelegateId = 2 } });
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(1, 2)).DoesNothing();
            A.CallTo(() => groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(1, 2, A<DateTime>._)).DoesNothing();

            // When
            var result = delegateGroupsController.GroupDelegatesRemove(model, 1, 2);

            // Then
            A.CallTo(() => groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(1, 2, A<DateTime>._)).MustHaveHappened();
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(1, 2)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("GroupDelegates");
        }
    }
}
