namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
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
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class DelegateGroupsControllerTests
    {
        private static readonly CustomPrompt ExpectedPrompt1 =
            CustomPromptsTestHelper.GetDefaultCustomPrompt(1, options: null, mandatory: true);

        private static readonly List<CustomPrompt> CustomPrompts = new List<CustomPrompt> { ExpectedPrompt1 };

        private readonly CentreCustomPrompts prompts =
            CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(CustomPrompts);

        private ICentreCustomPromptsService centreCustomPromptsService = null!;

        private IClockService clockService = null!;
        private DelegateGroupsController delegateGroupsController = null!;
        private IGroupsDataService groupsDataService = null!;
        private IGroupsService groupsService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;

        [SetUp]
        public void Setup()
        {
            centreCustomPromptsService = A.Fake<ICentreCustomPromptsService>();
            groupsDataService = A.Fake<IGroupsDataService>();
            groupsService = A.Fake<IGroupsService>();
            clockService = A.Fake<IClockService>();

            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(new List<Group>());
            A.CallTo(() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(A<int>._))
                .Returns(prompts);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "DelegateGroupsFilter";
            const string cookieValue = "LinkedToField|LinkedToField|0";

            delegateGroupsController = new DelegateGroupsController(
                    groupsDataService,
                    centreCustomPromptsService,
                    clockService,
                    groupsService
                )
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
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
        public void Index_with_no_query_parameters_uses_cookie_value_for_filterBy()
        {
            // When
            var result = delegateGroupsController.Index();

            // Then
            result.As<ViewResult>().Model.As<DelegateGroupsViewModel>().FilterBy.Should()
                .Be("LinkedToField|LinkedToField|0");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_filterBy()
        {
            // Given
            const string filterBy = "LinkedToField|LinkedToField|4";
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = delegateGroupsController.Index(filterBy: filterBy);

            // Then
            result.As<ViewResult>().Model.As<DelegateGroupsViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string? filterBy = "CLEAR";

            // When
            var result = delegateGroupsController.Index(filterBy: filterBy);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete("DelegateGroupsFilter")).MustHaveHappened();
            result.As<ViewResult>().Model.As<DelegateGroupsViewModel>().FilterBy.Should()
                .BeNull();
        }

        [Test]
        public void Index_with_null_filterBy_and_new_filter_query_parameter_add_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string? newFilterValue = "LinkedToField|LinkedToField|4";

            // When
            var result = delegateGroupsController.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("DelegateGroupsFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<DelegateGroupsViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_and_new_filter_value_query_parameter_sets_cookie()
        {
            // Given
            const string? filterBy = "CLEAR";
            const string? newFilterValue = "LinkedToField|LinkedToField|4";

            // When
            var result = delegateGroupsController.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("DelegateGroupsFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<DelegateGroupsViewModel>().FilterBy.Should()
                .Be(newFilterValue);
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
            A.CallTo(() => groupsDataService.GetGroupDelegates(1))
                .Returns(new List<GroupDelegate> { new GroupDelegate { DelegateId = 2 } });

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
            var model = new GroupDelegatesRemoveViewModel { ConfirmRemovalFromGroup = true, RemoveProgress = false };
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsDataService.GetGroupDelegates(1))
                .Returns(new List<GroupDelegate> { new GroupDelegate { DelegateId = 2 } });
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(1, 2)).DoesNothing();

            // When
            var result = delegateGroupsController.GroupDelegatesRemove(model, 1, 2);

            // Then
            A.CallTo(
                () => groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(A<int>._, A<int>._, A<DateTime>._, A<bool>._)
            ).MustNotHaveHappened();
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(1, 2)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("GroupDelegates");
        }

        [Test]
        public void GroupDelegatesRemove_should_call_remove_progress_if_checked()
        {
            // Given
            var model = new GroupDelegatesRemoveViewModel { ConfirmRemovalFromGroup = true, RemoveProgress = true };
            A.CallTo(() => groupsDataService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsDataService.GetGroupDelegates(1))
                .Returns(new List<GroupDelegate> { new GroupDelegate { DelegateId = 2 } });
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(1, 2)).DoesNothing();
            A.CallTo(() => groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(1, 2, A<DateTime>._, A<bool>._))
                .DoesNothing();

            // When
            var result = delegateGroupsController.GroupDelegatesRemove(model, 1, 2);

            // Then
            A.CallTo(() => groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(1, 2, A<DateTime>._, A<bool>._))
                .MustHaveHappened();
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(1, 2)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("GroupDelegates");
        }
    }
}
