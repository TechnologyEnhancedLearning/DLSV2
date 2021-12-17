namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AddDelegateToGroup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class AddDelegateToGroupControllerTests
    {
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

        private AddDelegateToGroupController addDelegateToGroupsController = null!;
        private CentreCustomPromptHelper centreCustomPromptHelper = null!;
        private IClockService clockService = null!;
        private IGroupsDataService groupsDataService = null!;
        private IGroupsService groupsService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IJobGroupsDataService jobGroupsDataService;
        private IUserDataService userDataService;

        [SetUp]
        public void Setup()
        {
            centreCustomPromptHelper = A.Fake<CentreCustomPromptHelper>();
            groupsDataService = A.Fake<IGroupsDataService>();
            groupsService = A.Fake<IGroupsService>();
            clockService = A.Fake<IClockService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            userDataService = A.Fake<IUserDataService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "AddDelegateToGroupFilter";
            const string cookieValue = "ActiveStatus|Active|false";

            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(
                JobGroupsTestHelper.GetDefaultJobGroupsAlphabetical()
            );

            A.CallTo(() => userDataService.GetDelegatesNotRegisteredForGroupByGroupId(A<int>._, A<int>._))
                .Returns(delegateUserCards);

            A.CallTo(() => groupsService.GetGroupName(A<int>._, A<int>._))
                .Returns("Group name");

            addDelegateToGroupsController = new AddDelegateToGroupController(
                    jobGroupsDataService,
                    userDataService,
                    centreCustomPromptHelper,
                    groupsService,
                    groupsDataService,
                    clockService
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
            var result = addDelegateToGroupsController.Index(groupId: 1);

            // Then
            result.As<ViewResult>().Model.As<AddDelegateToGroupViewModel>().FilterBy.Should()
                .Be("ActiveStatus|Active|false");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_filterBy()
        {
            // Given
            const string filterBy = "PasswordStatus|IsPasswordSet|true";
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = addDelegateToGroupsController.Index(groupId: 1, filterBy: filterBy);

            // Then
            result.As<ViewResult>().Model.As<AddDelegateToGroupViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string filterBy = "CLEAR";

            // When
            var result = addDelegateToGroupsController.Index(groupId: 1, filterBy: filterBy);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete("AddDelegateToGroupFilter")).MustHaveHappened();
            result.As<ViewResult>().Model.As<AddDelegateToGroupViewModel>().FilterBy.Should()
                .BeNull();
        }

        [Test]
        public void Index_with_null_filterBy_and_new_filter_query_parameter_adds_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string newFilterValue = "PasswordStatus|IsPasswordSet|true";

            // When
            var result = addDelegateToGroupsController.Index(groupId: 1, filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("AddDelegateToGroupFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<AddDelegateToGroupViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_and_new_filter_query_parameter_sets_new_cookie_value()
        {
            // Given
            const string filterBy = "CLEAR";
            const string newFilterValue = "PasswordStatus|IsPasswordSet|true";

            // When
            var result = addDelegateToGroupsController.Index(groupId: 1, filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("AddDelegateToGroupFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<AddDelegateToGroupViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }

        [Test]
        public void Index_with_no_filtering_should_default_to_Active_delegates()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies).Returns(A.Fake<IRequestCookieCollection>());

            // When
            var result = addDelegateToGroupsController.Index(groupId: 1);

            // Then
            result.As<ViewResult>().Model.As<AddDelegateToGroupViewModel>().FilterBy.Should()
                .Be("ActiveStatus|Active|true");
        }

        [Test]
        public void Add_delegate_to_group_returns_confirmation_view()
        {
            // Given
            var delegateUser = delegateUserCards[0];
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id))
                .Returns(delegateUser);

            A.CallTo(() => groupsService.GetGroupName(2, 1))
                .Returns("Group name");

            A.CallTo(
                () =>
                    groupsDataService.AddDelegateToGroup(
                        delegateUser.Id,
                        A<int>._,
                        A<DateTime>._,
                        0
                    )
            ).DoesNothing();

            A.CallTo(
                () =>
                    groupsService.EnrolDelegateOnGroupCourses(
                        delegateUser,
                        A<MyAccountDetailsData>._,
                        A<int>._,
                        1
                    )
            ).DoesNothing();

            // When
            var result = addDelegateToGroupsController.DelegateAddedToGroupConfirmation(2, 1);

            // Then
            result.As<ViewResult>().Model.Should().BeOfType<DelegateAddedToGroupConfirmationViewModel>();
        }

        [Test]
        public void Add_delegate_to_group_returns_not_found_with_an_incorrect_delegateId()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserById(1))
                .Returns(null);

            // When
            var result = addDelegateToGroupsController.DelegateAddedToGroupConfirmation(2, 1);

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
