namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
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

        private ICustomPromptsService customPromptsService = null!;

        private DelegateGroupsController delegateGroupsController = null!;
        private IGroupsDataService groupsDataService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;

        [SetUp]
        public void Setup()
        {
            customPromptsService = A.Fake<ICustomPromptsService>();
            groupsDataService = A.Fake<IGroupsDataService>();

            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(new List<Group>());
            A.CallTo(() => customPromptsService.GetCustomPromptsForCentreByCentreId(A<int>._))
                .Returns(prompts);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "DelegateGroupsFilter";
            const string cookieValue = "LinkedToField|LinkedToField|0";

            delegateGroupsController = new DelegateGroupsController(groupsDataService, customPromptsService)
                .WithMockHttpContextWithCookie(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockTempData(); ;
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
    }
}
