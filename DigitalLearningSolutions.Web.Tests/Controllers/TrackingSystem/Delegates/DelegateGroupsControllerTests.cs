namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
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

        private DelegateGroupsController delegateGroupsController = null!;
        private IGroupsService groupsService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;

        [SetUp]
        public void Setup()
        {
            centreCustomPromptsService = A.Fake<ICentreCustomPromptsService>();
            groupsService = A.Fake<IGroupsService>();

            A.CallTo(() => groupsService.GetGroupsForCentre(A<int>._)).Returns(new List<Group>());
            A.CallTo(() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(A<int>._))
                .Returns(prompts);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "DelegateGroupsFilter";
            const string cookieValue = "LinkedToField|LinkedToField|0";

            delegateGroupsController = new DelegateGroupsController(
                    centreCustomPromptsService,
                    groupsService
                )
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
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
        public void DeleteGroup_redirects_to_confirmation_if_group_has_delegates()
        {
            // Given
            A.CallTo(() => groupsService.GetGroupCentreId(A<int>._))
                .Returns(delegateGroupsController.User.GetCentreId());
            A.CallTo(() => groupsService.GetGroupDelegates(A<int>._))
                .Returns(new List<GroupDelegate> { new GroupDelegate() });
            const int groupId = 1;

            // When
            var result = delegateGroupsController.DeleteGroup(groupId, null);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName("ConfirmDeleteGroup")
                .WithRouteValue("groupId", groupId);
        }

        [Test]
        public void DeleteGroup_redirects_to_confirmation_if_group_has_courses()
        {
            // Given
            A.CallTo(() => groupsService.GetGroupCentreId(A<int>._))
                .Returns(delegateGroupsController.User.GetCentreId());
            A.CallTo(() => groupsService.GetUsableGroupCoursesForCentre(A<int>._, A<int>._))
                .Returns(new List<GroupCourse> { new GroupCourse() });
            const int groupId = 1;

            // When
            var result = delegateGroupsController.DeleteGroup(groupId, null);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName("ConfirmDeleteGroup")
                .WithRouteValue("groupId", groupId);
        }

        [Test]
        public void DeleteGroup_deletes_group_with_no_delegates_or_courses()
        {
            // Given
            A.CallTo(() => groupsService.GetGroupCentreId(A<int>._))
                .Returns(delegateGroupsController.User.GetCentreId());
            const int groupId = 1;

            // When
            var result = delegateGroupsController.DeleteGroup(groupId, null);

            // Then
            A.CallTo(() => groupsService.DeleteDelegateGroup(groupId, false)).MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void ConfirmDeleteGroup_with_deleteEnrolments_false_deletes_group_correctly()
        {
            // Given
            A.CallTo(() => groupsService.GetGroupCentreId(A<int>._))
                .Returns(delegateGroupsController.User.GetCentreId());
            var model = new ConfirmDeleteGroupViewModel
            {
                DeleteEnrolments = false,
                Confirm = true,
            };
            const int groupId = 1;

            // When
            var result = delegateGroupsController.ConfirmDeleteGroup(groupId, model);

            // Then
            A.CallTo(() => groupsService.DeleteDelegateGroup(groupId, false)).MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void ConfirmDeleteGroup_with_deleteEnrolments_true_deletes_group_correctly()
        {
            // Given
            A.CallTo(() => groupsService.GetGroupCentreId(A<int>._))
                .Returns(delegateGroupsController.User.GetCentreId());
            var model = new ConfirmDeleteGroupViewModel
            {
                DeleteEnrolments = true,
                Confirm = true,
            };
            const int groupId = 1;

            // When
            var result = delegateGroupsController.ConfirmDeleteGroup(groupId, model);

            // Then
            A.CallTo(() => groupsService.DeleteDelegateGroup(groupId, true)).MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void EditDelegatesGroupDescription_should_redirect_to_index_action()
        {
            // Given
            const int groupId = 103;
            const int centreId = 2;
            var model = new EditDelegateGroupDescriptionViewModel
            {
                Description = "Test Description",
            };

            A.CallTo(
                () => groupsService.UpdateGroupDescription(
                    groupId,
                    centreId,
                    model.Description
                )
            ).DoesNothing();

            // When
            var result = delegateGroupsController.EditDescription(model, groupId);

            // Then
            A.CallTo(
                () => groupsService.UpdateGroupDescription(
                    groupId,
                    centreId,
                    model.Description
                )
            );

            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void EditGroupName_should_redirect_to_index_action_when_update_is_successful()
        {
            // Given
            const int groupId = 103;
            const int centreId = 2;
            var model = new EditGroupNameViewModel
            {
                GroupName = "Test Group Name",
            };

            A.CallTo(
                () => groupsService.UpdateGroupName(
                    groupId,
                    centreId,
                    model.GroupName
                )
            ).DoesNothing();

            // When
            var result = delegateGroupsController.EditGroupName(model, groupId);

            // Then
            A.CallTo(
                    () => groupsService.UpdateGroupName(
                        groupId,
                        centreId,
                        model.GroupName
                    )
                )
                .MustHaveHappened();

            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void EditGroupName_should_redirect_to_not_found_page_when_linked_to_field_is_not_zero()
        {
            // Given
            var model = new EditGroupNameViewModel { GroupName = "Test Group Name" };
            A.CallTo(() => groupsService.GetGroupAtCentreById(1, 2))
                .Returns(new Group { LinkedToField = 1 });

            // When
            var result = delegateGroupsController.EditGroupName(1, null);

            // Them
            A.CallTo(() => groupsService.GetGroupAtCentreById(1, 2)).MustHaveHappened();
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void EditGroupName_should_not_update_name_when_linked_to_field_is_not_zero()
        {
            // Given
            var model = new EditGroupNameViewModel { GroupName = "Test Group Name" };
            A.CallTo(() => groupsService.GetGroupAtCentreById(1, 2))
                .Returns(new Group { LinkedToField = 1 });

            // When
            var result = delegateGroupsController.EditGroupName(model, 1);

            // Them
            A.CallTo(() => groupsService.GetGroupAtCentreById(1, 2)).MustHaveHappened();
            A.CallTo(() => groupsService.UpdateGroupName(1, 2, model.GroupName))
                .MustNotHaveHappened();

            result.Should().BeNotFoundResult();
        }
    }
}
