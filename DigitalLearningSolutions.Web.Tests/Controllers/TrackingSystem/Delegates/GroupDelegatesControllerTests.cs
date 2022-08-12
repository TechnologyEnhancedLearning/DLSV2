namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
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
                JobGroupId = 1,
            },
        };

        private GroupDelegatesController groupDelegatesController = null!;
        private IGroupsService groupsService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IJobGroupsService jobGroupsService = null!;
        private PromptsService promptsService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            promptsService = A.Fake<PromptsService>();
            groupsService = A.Fake<IGroupsService>();
            jobGroupsService = A.Fake<IJobGroupsService>();
            userService = A.Fake<IUserService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieValue = "ActiveStatus|Active|false";

            groupDelegatesController = new GroupDelegatesController(
                    jobGroupsService,
                    userService,
                    promptsService,
                    groupsService,
                    searchSortFilterPaginateService
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
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void SelectDelegate_calls_expected_methods_and_returns_view()
        {
            // When
            var result = groupDelegatesController.SelectDelegate(1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.GetDelegatesNotRegisteredForGroupByGroupId(A<int>._, A<int>._))
                    .MustHaveHappened();
                A.CallTo(() => jobGroupsService.GetJobGroupsAlphabetical())
                    .MustHaveHappened();
                A.CallTo(() => groupsService.GetGroupName(A<int>._, A<int>._)).MustHaveHappened();
                A.CallTo(
                    () => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                        A<IEnumerable<DelegateUserCard>>._,
                        A<SearchSortFilterAndPaginateOptions>._
                    )
                ).MustHaveHappened();
                A.CallTo(
                        () => httpResponse.Cookies.Append(
                            AddGroupDelegateFilterCookieName,
                            A<string>._,
                            A<CookieOptions>._
                        )
                    )
                    .MustHaveHappened();
                result.Should().BeViewResult().WithDefaultViewName();
            }
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
                        1,
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

        [Test]
        public void RemoveGroupDelegate_get_returns_NotFound_if_the_delegate_is_not_in_the_group()
        {
            // Given
            const int groupId = 1;
            const int delegateId = 2;
            const int delegateIdNotInGroup = 3;

            A.CallTo(() => groupsService.GetGroupName(groupId, 2)).Returns("Group");
            A.CallTo(() => groupsService.GetGroupDelegates(groupId))
                .Returns(new List<GroupDelegate> { new GroupDelegate { DelegateId = delegateId } });

            // When
            var result = groupDelegatesController.RemoveGroupDelegate(
                groupId,
                delegateIdNotInGroup,
                new ReturnPageQuery()
            );

            // Then
            result.Should().BeNotFoundResult();

            A.CallTo(() => groupsService.RemoveDelegateFromGroup(A<int>._, A<int>._, A<bool>._))
                .MustNotHaveHappened();
        }
    }
}
