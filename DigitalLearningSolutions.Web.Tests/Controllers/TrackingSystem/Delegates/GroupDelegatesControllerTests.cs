namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class GroupDelegatesControllerTests
    {
        private const string AddGroupDelegateFilterCookieName = "AddGroupDelegateFilter";

        private readonly List<DelegateUserCard> delegateUserCards = new List<DelegateUserCard>
        {
            new DelegateUserCard
            {
                FirstName = "A",
                LastName = "Test",
                EmailAddress = null,
                CandidateNumber = "TT95",
                Answer1 = "xxxx",
                Answer2 = "xxxxxxxxx",
                Answer3 = null,
                Answer4 = null,
                Answer5 = null,
                Answer6 = null,
                Active = true,
                AliasId = null,
                JobGroupId = 1,
            },
            new DelegateUserCard
            {
                FirstName = "Fake",
                LastName = "Person",
                EmailAddress = "Test@Test",
                CandidateNumber = "TU67",
                Answer1 = null,
                Answer2 = null,
                Answer3 = null,
                Answer4 = null,
                Answer5 = null,
                Answer6 = null,
                Active = true,
                AliasId = null,
                JobGroupId = 1,
            },
        };

        private PromptsService promptsService = null!;

        private GroupDelegatesController groupDelegatesController = null!;
        private IGroupsService groupsService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IJobGroupsService jobGroupsService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            promptsService = A.Fake<PromptsService>();
            groupsService = A.Fake<IGroupsService>();
            jobGroupsService = A.Fake<IJobGroupsService>();
            userService = A.Fake<IUserService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieValue = "ActiveStatus|Active|false";

            A.CallTo(() => jobGroupsService.GetJobGroupsAlphabetical()).Returns(
                JobGroupsTestHelper.GetDefaultJobGroupsAlphabetical()
            );

            A.CallTo(() => userService.GetDelegatesNotRegisteredForGroupByGroupId(A<int>._, A<int>._))
                .Returns(delegateUserCards);

            A.CallTo(() => groupsService.GetGroupName(A<int>._, A<int>._))
                .Returns("Group name");

            groupDelegatesController = new GroupDelegatesController(
                    jobGroupsService,
                    userService,
                    promptsService,
                    groupsService
                )
                .WithMockHttpContext(httpRequest, AddGroupDelegateFilterCookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_returns_view_result_with_correct_group_id_for_centre()
        {
            // Given
            A.CallTo(() => groupsService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsService.GetGroupDelegates(1)).Returns(new List<GroupDelegate>());

            // When
            var result = groupDelegatesController.Index(1);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithDefaultViewName();
                result.As<ViewResult>().Model.As<GroupDelegatesViewModel>().NavViewModel.GroupName.Should().Be("Group");
                result.As<ViewResult>().Model.As<GroupDelegatesViewModel>().NavViewModel.CurrentPage.Should()
                    .Be(DelegateGroupPage.Delegates);
            }
        }

        [Test]
        public void SelectDelegate_with_no_query_parameters_uses_cookie_value_for_filterBy()
        {
            // When
            var result = groupDelegatesController.SelectDelegate(1);

            // Then
            result.As<ViewResult>().Model.As<AddGroupDelegateViewModel>().FilterBy.Should()
                .Be("ActiveStatus|Active|false");
        }

        [Test]
        public void SelectDelegate_with_query_parameters_uses_query_parameter_value_for_filterBy()
        {
            // Given
            const string filterBy = "PasswordStatus|IsPasswordSet|true";
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = groupDelegatesController.SelectDelegate(1, filterBy: filterBy);

            // Then
            result.As<ViewResult>().Model.As<AddGroupDelegateViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void SelectDelegate_with_CLEAR_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string filterBy = "CLEAR";

            // When
            var result = groupDelegatesController.SelectDelegate(1, filterBy: filterBy);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Delete(AddGroupDelegateFilterCookieName)).MustHaveHappened();
                result.As<ViewResult>().Model.As<AddGroupDelegateViewModel>().FilterBy.Should()
                    .BeNull();
            }
        }

        [Test]
        public void SelectDelegate_with_null_filterBy_and_new_filter_query_parameter_adds_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string newFilterValue = "PasswordStatus|IsPasswordSet|true";

            // When
            var result = groupDelegatesController.SelectDelegate(1, filterBy: filterBy, filterValue: newFilterValue);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => httpResponse.Cookies.Append(
                            AddGroupDelegateFilterCookieName,
                            newFilterValue,
                            A<CookieOptions>._
                        )
                    )
                    .MustHaveHappened();
                result.As<ViewResult>().Model.As<AddGroupDelegateViewModel>().FilterBy.Should()
                    .Be(newFilterValue);
            }
        }

        [Test]
        public void SelectDelegate_with_CLEAR_filterBy_and_new_filter_query_parameter_sets_new_cookie_value()
        {
            // Given
            const string filterBy = "CLEAR";
            const string newFilterValue = "PasswordStatus|IsPasswordSet|true";

            // When
            var result = groupDelegatesController.SelectDelegate(1, filterBy: filterBy, filterValue: newFilterValue);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => httpResponse.Cookies.Append(
                            AddGroupDelegateFilterCookieName,
                            newFilterValue,
                            A<CookieOptions>._
                        )
                    )
                    .MustHaveHappened();
                result.As<ViewResult>().Model.As<AddGroupDelegateViewModel>().FilterBy.Should()
                    .Be(newFilterValue);
            }
        }

        [Test]
        public void SelectDelegate_with_no_filtering_should_not_have_a_filter_set()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies).Returns(A.Fake<IRequestCookieCollection>());

            // When
            var result = groupDelegatesController.SelectDelegate(1);

            // Then
            result.As<ViewResult>().Model.As<AddGroupDelegateViewModel>().FilterBy.Should()
                .BeNull();
        }

        [Test]
        public void AddDelegate_returns_confirmation_view()
        {
            // Given
            var delegateUser = delegateUserCards[0];
            A.CallTo(() => userService.GetDelegateUserById(delegateUser.Id))
                .Returns(delegateUser);

            A.CallTo(() => groupsService.GetGroupName(2, 1))
                .Returns("Group name");

            A.CallTo(
                () =>
                    groupsService.AddDelegateToGroupAndEnrolOnGroupCourses(
                        A<int>._,
                        delegateUser,
                        0
                    )
            ).DoesNothing();

            // When
            var result = groupDelegatesController.AddDelegate(2, 1);

            // Then
            result.Should().BeRedirectToActionResult()
                .WithActionName(nameof(GroupDelegatesController.ConfirmDelegateAdded));
        }
        
        [Test]
        public void RemoveGroupDelegate_should_call_remove_progress_but_keep_started_enrolments_if_unchecked()
        {
            // Given
            var model = new RemoveGroupDelegateViewModel
                { ConfirmRemovalFromGroup = true, RemoveStartedEnrolments = false };

            const int groupId = 44;
            const int delegateId = 3274;

            A.CallTo(() => groupsService.GetGroupName(groupId, 2)).Returns("Group");
            A.CallTo(() => groupsService.GetGroupDelegates(groupId))
                .Returns(new List<GroupDelegate> { new GroupDelegate { DelegateId = delegateId } });
            A.CallTo(
                () => groupsService.RemoveDelegateFromGroup(groupId, delegateId, model.RemoveStartedEnrolments)
            ).DoesNothing();

            // When
            var result = groupDelegatesController.RemoveGroupDelegate(model, groupId, delegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => groupsService.RemoveDelegateFromGroup(groupId, delegateId, model.RemoveStartedEnrolments)
                ).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
        }

        [Test]
        public void RemoveGroupDelegate_should_call_remove_progress_if_checked()
        {
            // Given
            var model = new RemoveGroupDelegateViewModel
                { ConfirmRemovalFromGroup = true, RemoveStartedEnrolments = true };
            A.CallTo(() => groupsService.GetGroupName(1, 2)).Returns("Group");
            A.CallTo(() => groupsService.GetGroupDelegates(1))
                .Returns(new List<GroupDelegate> { new GroupDelegate { DelegateId = 2 } });
            A.CallTo(() => groupsService.RemoveDelegateFromGroup(1, 2, A<bool>._))
                .DoesNothing();

            // When
            var result = groupDelegatesController.RemoveGroupDelegate(model, 1, 2);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => groupsService.RemoveDelegateFromGroup(1, 2, A<bool>._))
                    .MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
        }
    }
}
