namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class GroupCoursesControllerTests
    {
        private ICourseService courseService = null!;

        private GroupCoursesController groupCoursesController = null!;
        private IGroupsService groupsService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            groupsService = A.Fake<IGroupsService>();
            userService = A.Fake<IUserService>();
            courseService = A.Fake<ICourseService>();

            A.CallTo(() => groupsService.GetGroupsForCentre(A<int>._)).Returns(new List<Group>());

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "DelegateGroupsFilter";
            const string cookieValue = "LinkedToField|LinkedToField|0";

            groupCoursesController = new GroupCoursesController(
                    userService,
                    courseService,
                    groupsService
                )
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void GroupCourses_Index_returns_not_found_with_incorrect_group_id_for_centre()
        {
            // Given
            A.CallTo(() => groupsService.GetGroupName(1, 2)).Returns(null);

            // When
            var result = groupCoursesController.Index(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void GroupCourses_Index_returns_view_result_with_correct_group_id_for_centre()
        {
            // Given
            A.CallTo(() => groupsService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsService.GetUsableGroupCoursesForCentre(1, 2)).Returns(new List<GroupCourse>());

            // When
            var result = groupCoursesController.Index(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            result.As<ViewResult>().Model.As<GroupCoursesViewModel>().NavViewModel.GroupName.Should().Be("Group");
            result.As<ViewResult>().Model.As<GroupCoursesViewModel>().NavViewModel.CurrentPage.Should()
                .Be(DelegateGroupPage.Courses);
        }

        [Test]
        public void RemoveGroupCourse_with_invalid_model_returns_view_with_error()
        {
            // Given
            var groupId = 1;
            var groupCustomisationId = 25;
            var model = new RemoveGroupCourseViewModel();
            groupCoursesController.ModelState.AddModelError("Confirm", "Is Invalid.");

            // When
            var result = groupCoursesController.RemoveGroupCourse(groupId, groupCustomisationId, model);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
            groupCoursesController.ModelState.IsValid.Should().BeFalse();
            A.CallTo(() => groupsService.RemoveGroupCourseAndRelatedProgress(A<int>._, A<int>._, A<bool>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void RemoveGroupCourse_with_valid_model_calls_remove_service_and_redirects()
        {
            // Given
            var groupId = 1;
            var groupCustomisationId = 25;
            var model = new RemoveGroupCourseViewModel
            {
                Confirm = true,
            };

            // When
            var result = groupCoursesController.RemoveGroupCourse(groupId, groupCustomisationId, model);

            // Then
            A.CallTo(() => groupsService.RemoveGroupCourseAndRelatedProgress(groupCustomisationId, groupId, false))
                .MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult()
                .WithActionName("Index")
                .WithRouteValue("groupId", groupId);
        }

        [Test]
        public void AddCourseToGroup_with_invalid_model_does_not_call_add_course_service_and_returns_view()
        {
            // Given
            const int groupId = 1;
            const int customisationId = 25;
            var formData = new AddCourseFormData
            {
                MonthsToComplete = "invalidString",
            };
            groupCoursesController.ModelState.AddModelError(nameof(AddCourseFormData.MonthsToComplete), "Is Invalid.");
            A.CallTo(() => courseService.GetCourseCategoryId(customisationId, ControllerContextHelper.CentreId))
                .Returns(1);

            // When
            var result = groupCoursesController.AddCourseToGroup(formData, groupId, customisationId);

            // Then
            A.CallTo(() => groupsService.AddCourseToGroup(A<int>._, A<int>._, A<int>._, A<int>._, A<bool>._, A<int?>._))
                .MustNotHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void AddCourseToGroup_with_valid_model_calls_add_course_service_and_returns_expected_view()
        {
            // Given
            const int groupId = 1;
            const int customisationId = 25;
            var formData = new AddCourseFormData
            {
                CohortLearners = false,
                SupervisorId = 1,
                MonthsToComplete = "1",
            };

            // When
            var result = groupCoursesController.AddCourseToGroup(formData, groupId, customisationId);

            // Then
            A.CallTo(
                    () => groupsService.AddCourseToGroup(
                        groupId,
                        customisationId,
                        1,
                        ControllerContextHelper.AdminId,
                        formData.CohortLearners,
                        formData.SupervisorId
                    )
                )
                .MustHaveHappenedOnceExactly();
            result.Should().BeViewResult().WithViewName("AddCourseToGroupConfirmation");
        }
    }
}
