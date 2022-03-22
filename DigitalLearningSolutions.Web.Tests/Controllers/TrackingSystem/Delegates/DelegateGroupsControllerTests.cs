namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NUnit.Framework;

    public class DelegateGroupsControllerTests
    {
        private const string CookieName = "DelegateGroupsFilter";
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private DelegateGroupsController delegateGroupsController = null!;
        private IGroupsService groupsService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            groupsService = A.Fake<IGroupsService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieValue = "LinkedToField|LinkedToField|0";

            delegateGroupsController = new DelegateGroupsController(
                    centreRegistrationPromptsService,
                    groupsService,
                    searchSortFilterPaginateService
                )
                .WithMockHttpContext(httpRequest, CookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_calls_expected_methods_and_returns_view()
        {
            // When
            var result = delegateGroupsController.Index();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => groupsService.GetGroupsForCentre(A<int>._)).MustHaveHappened();
                A.CallTo(
                    () => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                        A<IEnumerable<Group>>._,
                        A<SearchSortFilterAndPaginateOptions>._
                    )
                ).MustHaveHappened();
                A.CallTo(
                        () => httpResponse.Cookies.Append(
                            CookieName,
                            A<string>._,
                            A<CookieOptions>._
                        )
                    )
                    .MustHaveHappened();
                result.Should().BeViewResult().WithDefaultViewName();
            }
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
            using (new AssertionScope())
            {
                A.CallTo(
                    () => groupsService.UpdateGroupDescription(
                        groupId,
                        centreId,
                        model.Description
                    )
                ).MustHaveHappenedOnceExactly();

                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
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
            A.CallTo(() => groupsService.GetGroupAtCentreById(1, 2))
                .Returns(new Group { LinkedToField = 1 });

            // When
            var result = delegateGroupsController.EditGroupName(1, null);

            // Then
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

            // Then
            A.CallTo(() => groupsService.GetGroupAtCentreById(1, 2)).MustHaveHappened();
            A.CallTo(() => groupsService.UpdateGroupName(1, 2, model.GroupName))
                .MustNotHaveHappened();

            result.Should().BeNotFoundResult();
        }

        [Test]
        public void GenerateGroups_GET_should_populate_registration_field_options_correctly()
        {
            // Given
            const string customPromptName1 = "Role";
            const string customPromptName2 = "Team";
            const int centreId = 2;

            var customPromptSelectListItem1 = new SelectListItem(customPromptName1, "1");
            var customPromptSelectListItem2 = new SelectListItem(customPromptName2, "2");
            var jobGroupSelectListItem = new SelectListItem("Job group", "7");
            var registrationFieldOptions = new List<SelectListItem>
                { customPromptSelectListItem1, customPromptSelectListItem2, jobGroupSelectListItem };

            var customPrompt1 = new CentreRegistrationPrompt(1, 1, customPromptName1, "Test", false);
            var customPrompt2 = new CentreRegistrationPrompt(2, 2, customPromptName2, "Test", false);
            var customPrompts = new List<CentreRegistrationPrompt> { customPrompt1, customPrompt2 };

            A.CallTo(
                    () => centreRegistrationPromptsService.GetCentreRegistrationPromptsThatHaveOptionsByCentreId(
                        centreId
                    )
                )
                .Returns(customPrompts);

            // When
            var result = delegateGroupsController.GenerateGroups();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => centreRegistrationPromptsService.GetCentreRegistrationPromptsThatHaveOptionsByCentreId(
                            A<int>._
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                result.Should().BeViewResult().ModelAs<GenerateGroupsViewModel>().RegistrationFieldOptions.Should()
                    .BeEquivalentTo(registrationFieldOptions);
            }
        }

        [Test]
        public void GenerateGroups_GET_should_append_duplicate_registration_prompt_options_with_prompt_number()
        {
            // Given
            const string customPromptName = "Role";
            const int centreId = 2;

            var customPromptSelectListItem1 = new SelectListItem($"{customPromptName} (Prompt 1)", "1");
            var customPromptSelectListItem2 = new SelectListItem($"{customPromptName} (Prompt 2)", "2");
            var jobGroupSelectListItem = new SelectListItem("Job group", "7");
            var registrationFieldOptions = new List<SelectListItem>
                { customPromptSelectListItem1, customPromptSelectListItem2, jobGroupSelectListItem };

            var customPrompt1 = new CentreRegistrationPrompt(1, 1, customPromptName, "Test", false);
            var customPrompt2 = new CentreRegistrationPrompt(2, 2, customPromptName, "Test", false);
            var customPrompts = new List<CentreRegistrationPrompt> { customPrompt1, customPrompt2 };

            A.CallTo(
                    () => centreRegistrationPromptsService.GetCentreRegistrationPromptsThatHaveOptionsByCentreId(
                        centreId
                    )
                )
                .Returns(customPrompts);

            // When
            var result = delegateGroupsController.GenerateGroups();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => centreRegistrationPromptsService.GetCentreRegistrationPromptsThatHaveOptionsByCentreId(
                            A<int>._
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                result.Should().BeViewResult().ModelAs<GenerateGroupsViewModel>().RegistrationFieldOptions.Should()
                    .BeEquivalentTo(registrationFieldOptions);
            }
        }

        [Test]
        public void GenerateGroups_POST_should_call_service_and_redirect_to_index()
        {
            // Given
            const string groupNamePrefix = "Role";

            var registrationField = RegistrationField.CentreRegistrationField1;
            var customPromptSelectListItem = new SelectListItem(groupNamePrefix, registrationField.Id.ToString());
            var jobGroup = new SelectListItem("Job group", "2");
            var registrationFieldOptions = new List<SelectListItem> { customPromptSelectListItem, jobGroup };

            var customPrompt1 = new CentreRegistrationPrompt(1, 1, groupNamePrefix, "Test", false);
            var customPrompts = new List<CentreRegistrationPrompt> { customPrompt1 };

            var model = new GenerateGroupsViewModel(
                registrationFieldOptions,
                registrationField.Id,
                false,
                true,
                false,
                true,
                false
            );

            A.CallTo(
                    () => centreRegistrationPromptsService.GetCentreRegistrationPromptsThatHaveOptionsByCentreId(
                        A<int>._
                    )
                )
                .Returns(customPrompts);

            A.CallTo(() => groupsService.GenerateGroupsFromRegistrationField(A<GroupGenerationDetails>._))
                .DoesNothing();

            // When
            var result = delegateGroupsController.GenerateGroups(model);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => groupsService.GenerateGroupsFromRegistrationField(
                            A<GroupGenerationDetails>.That.Matches(
                                gd =>
                                    gd.AdminId == 7 &&
                                    gd.CentreId == 2 &&
                                    gd.RegistrationField.Equals(registrationField) &&
                                    gd.PrefixGroupName == model.PrefixGroupName &&
                                    gd.PopulateExisting == model.PopulateExisting &&
                                    gd.SyncFieldChanges == model.SyncFieldChanges &&
                                    gd.AddNewRegistrants == model.AddNewRegistrants &&
                                    gd.PopulateExisting == model.PopulateExisting
                            )
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
        }

        [Test]
        public void GenerateGroups_POST_should_not_call_service_if_model_state_is_invalid()
        {
            // Given
            var model = new GenerateGroupsViewModel();
            delegateGroupsController.ModelState.AddModelError("RegistrationFieldOptionId", "test error");

            A.CallTo(() => groupsService.GenerateGroupsFromRegistrationField(A<GroupGenerationDetails>._))
                .DoesNothing();

            // When
            var result = delegateGroupsController.GenerateGroups(model);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => groupsService.GenerateGroupsFromRegistrationField(A<GroupGenerationDetails>._))
                    .MustNotHaveHappened();
                result.Should().BeViewResult().ModelAs<GenerateGroupsViewModel>();
                delegateGroupsController.ModelState.IsValid.Should().BeFalse();
            }
        }

        [Test]
        public void GenerateGroups_POST_should_not_call_service_if_selected_field_is_a_free_text_field()
        {
            // Given
            var customPrompt1 = new CentreRegistrationPrompt(1, 2, "Role", "Test", false);
            var customPrompts = new List<CentreRegistrationPrompt> { customPrompt1 };

            var registrationField = RegistrationField.CentreRegistrationField3;

            var model = new GenerateGroupsViewModel(
                new List<SelectListItem>(),
                registrationField.Id,
                false,
                true,
                false,
                true,
                false
            );

            A.CallTo(
                    () => centreRegistrationPromptsService.GetCentreRegistrationPromptsThatHaveOptionsByCentreId(
                        A<int>._
                    )
                )
                .Returns(customPrompts);

            A.CallTo(() => groupsService.GenerateGroupsFromRegistrationField(A<GroupGenerationDetails>._))
                .DoesNothing();

            // When
            var result = delegateGroupsController.GenerateGroups(model);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => groupsService.GenerateGroupsFromRegistrationField(A<GroupGenerationDetails>._))
                    .MustNotHaveHappened();
                result.Should().BeStatusCodeResult();
            }
        }
    }
}
