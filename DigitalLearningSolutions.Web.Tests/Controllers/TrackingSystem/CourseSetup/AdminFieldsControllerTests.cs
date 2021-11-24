namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class AdminFieldsControllerTests
    {
        private readonly ICourseAdminFieldsDataService courseAdminFieldsDataService =
            A.Fake<ICourseAdminFieldsDataService>();

        private readonly ICourseAdminFieldsService courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
        private readonly ICourseService courseService = A.Fake<ICourseService>();
        private AdminFieldsController controller = null!;

        private static IEnumerable<TestCaseData> AddAnswerModelErrorTestData
        {
            get
            {
                yield return new TestCaseData(
                    new string('x', 1000),
                    "xx",
                    "The complete list of answers must be 1000 characters or fewer (0 characters remaining for the new answer, 2 characters were entered)"
                ).SetName("Error_message_shows_zero_characters_remaining_if_options_string_is_at_max_length");
                yield return new TestCaseData(
                    new string('x', 998),
                    "xx",
                    "The complete list of answers must be 1000 characters or fewer (0 characters remaining for the new answer, 2 characters were entered)"
                ).SetName(
                    "Error_message_shows_zero_characters_remaining_if_options_string_is_two_less_than_max_length"
                );
                yield return new TestCaseData(
                    new string('x', 996),
                    "xxxx",
                    "The complete list of answers must be 1000 characters or fewer (2 characters remaining for the new answer, 4 characters were entered)"
                ).SetName("Error_message_shows_two_less_than_number_of_characters_remaining_if_possible_to_add_answer");
            }
        }

        [SetUp]
        public void Setup()
        {
            controller = new AdminFieldsController(
                    courseAdminFieldsService,
                    courseAdminFieldsDataService
                )
                .WithDefaultContext()
                .WithMockUser(true, 101)
                .WithMockTempData();
        }

        [Test]
        public void AdminFields_returns_AdminFields_page_when_appropriate_course_found_and_clears_TempData()
        {
            // Given
            var samplePrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, "System Access Granted", "Yes\r\nNo");
            var customPrompts = new List<CustomPrompt> { samplePrompt1 };
            A.CallTo(() => courseAdminFieldsService.GetCustomPromptsForCourse(A<int>._))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFields(customPrompts));
            controller.TempData.Set(samplePrompt1);

            // When
            var result = controller.Index(1);

            // Then
            controller.TempData.Peek<CustomPrompt>().Should().BeNull();
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<AdminFieldsViewModel>();
        }

        [Test]
        public void PostEditAdminField_save_calls_correct_methods()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, "Test", "Options");
            const string action = "save";

            A.CallTo(
                () => courseAdminFieldsService.UpdateCustomPromptForCourse(
                    1,
                    1,
                    "Options"
                )
            ).DoesNothing();

            // When
            var result = controller.EditAdminField(1, model, action);

            // Then
            A.CallTo(
                () => courseAdminFieldsService.UpdateCustomPromptForCourse(
                    1,
                    1,
                    "Options"
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void PostEditAdminField_add_configures_new_answer()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, "Test", "Options");
            const string action = "addPrompt";

            A.CallTo(
                () => courseAdminFieldsService.UpdateCustomPromptForCourse(
                    1,
                    1,
                    "Test"
                )
            ).DoesNothing();

            // When
            var result = controller.EditAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditAdminFieldViewModel>();
                AssertNumberOfConfiguredAnswersOnView(result, 2);
            }
        }

        [Test]
        public void PostEditAdminField_delete_removes_configured_answer()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, "Test", "Test\r\nAnswer");
            const string action = "delete0";

            // When
            var result = controller.EditAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditAdminFieldViewModel>();
                AssertNumberOfConfiguredAnswersOnView(result, 1);
            }
        }

        [Test]
        public void PostAdminField_bulk_sets_up_temp_data_and_redirects()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, "Test", "Options");
            const string action = "bulk";

            // When
            var result = controller.EditAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                AssertEditTempDataIsExpected(model);
                result.Should().BeRedirectToActionResult().WithActionName("EditAdminFieldAnswersBulk");
            }
        }

        [Test]
        public void PostEditAdminField_returns_error_with_unexpected_action()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, "Test", "Options");
            const string action = "deletetest";

            // When
            var result = controller.EditAdminField(1, model, action);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void EditAdminFieldAnswersBulk_updates_temp_data_and_redirects_to_edit()
        {
            // Given
            var inputViewModel = new BulkAdminFieldAnswersViewModel("Test\r\nAnswer");
            var initialEditViewModel = new EditAdminFieldViewModel(1, "Test", "Test");
            var expectedViewModel = new EditAdminFieldViewModel(1, "Test", "Test\r\nAnswer");
            var initialTempData = new EditAdminFieldData(initialEditViewModel);

            controller.TempData.Set(initialTempData);

            A.CallTo(() => courseService.VerifyAdminUserCanAccessCourse(A<int>._, A<int>._, A<int>._))
                .Returns(true);

            // When
            var result = controller.EditAdminFieldAnswersBulk(1, 1, inputViewModel);

            // Then
            using (new AssertionScope())
            {
                AssertEditTempDataIsExpected(expectedViewModel);
                result.Should().BeRedirectToActionResult().WithActionName("EditAdminField");
            }
        }

        [Test]
        public void AddAdminFieldNew_sets_new_temp_data()
        {
            // When
            var result = controller.AddAdminFieldNew(1);

            // Then
            controller.TempData.Peek<AddAdminFieldData>().Should().NotBeNull();
            result.Should().BeRedirectToActionResult().WithActionName("AddAdminField");
        }

        [Test]
        public void AddAdminField_post_updates_temp_data_and_redirects()
        {
            var expectedPromptModel = new AddAdminFieldViewModel();
            var initialTempData = new AddAdminFieldData(expectedPromptModel);
            controller.TempData.Set(initialTempData);

            // When
            var result = controller.AddAdminField(1);

            // Then
            AssertAddTempDataIsExpected(expectedPromptModel);
            result.As<ViewResult>().Model.Should().BeOfType<AddAdminFieldViewModel>();
        }

        [Test]
        public void AddAdminField_save_redirects_to_index()
        {
            // Given
            var model = new AddAdminFieldViewModel(1, "Test");
            const string action = "save";
            var initialTempData = new AddAdminFieldData(model);
            controller.TempData.Set(initialTempData);

            A.CallTo(
                () => courseAdminFieldsService.AddCustomPromptToCourse(
                    100,
                    1,
                    "Test"
                )
            ).Returns(true);

            // When
            var result = controller.AddAdminField(100, model, action);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void AddAdminField_save_redirects_successfully_without_answers_configured()
        {
            // Given
            var model = new AddAdminFieldViewModel(1, null);
            const string action = "save";
            var initialTempData = new AddAdminFieldData(model);
            controller.TempData.Set(initialTempData);

            A.CallTo(
                () => courseAdminFieldsService.AddCustomPromptToCourse(
                    100,
                    1,
                    null
                )
            ).Returns(true);

            // When
            var result = controller.AddAdminField(100, model, action);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void AddAdminField_calls_service_and_redirects_to_error_on_failure()
        {
            // Given
            var model = new AddAdminFieldViewModel(1, "Test");
            const string action = "save";
            var initialTempData = new AddAdminFieldData(model);
            controller.TempData.Set(initialTempData);

            A.CallTo(
                () => courseAdminFieldsService.AddCustomPromptToCourse(
                    100,
                    1,
                    "Test"
                )
            ).Returns(false);

            // When
            var result = controller.AddAdminField(100, model, action);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeStatusCodeResult().WithStatusCode(500);
            }
        }

        [Test]
        public void AddAdminField_add_configures_new_answer_and_updates_temp_data()
        {
            var initialViewModel = new AddAdminFieldViewModel(1, "Test", "Answer");
            var initialTempData = new AddAdminFieldData(initialViewModel);
            controller.TempData.Set(initialTempData);

            var expectedViewModel = new AddAdminFieldViewModel(1, "Test\r\nAnswer");
            const string action = "addPrompt";

            // When
            var result =
                controller.AddAdminField(1, initialViewModel, action);

            // Then
            using (new AssertionScope())
            {
                AssertAddTempDataIsExpected(expectedViewModel);
                result.As<ViewResult>().Model.Should().BeOfType<AddAdminFieldViewModel>();
                AssertNumberOfConfiguredAnswersOnView(result, 2);
            }
        }

        [Test]
        public void AddAdminField_adds_answer_without_admin_field_selected()
        {
            var initialViewModel = new AddAdminFieldViewModel(null, null, "Answer");
            var initialTempData = new AddAdminFieldData(initialViewModel);
            controller.TempData.Set(initialTempData);

            var expectedViewModel = new AddAdminFieldViewModel(null, "Answer");
            const string action = "addPrompt";

            // When
            controller.AddAdminField(1, initialViewModel, action);

            // Then
            using (new AssertionScope())
            {
                AssertAddTempDataIsExpected(expectedViewModel);
            }
        }

        [Test]
        [TestCaseSource(
            typeof(AdminFieldsControllerTests),
            nameof(AddAnswerModelErrorTestData)
        )]
        public void AddAdminField_adds_model_error_if_new_answer_surpasses_character_limit(
            string optionsString,
            string newAnswerInput,
            string expectedErrorMessage
        )
        {
            // Given
            var initialViewModel = new AddAdminFieldViewModel(1, optionsString, newAnswerInput);
            var initialTempData = new AddAdminFieldData(initialViewModel);
            controller.TempData.Set(initialTempData);
            const string action = "addPrompt";

            // When
            var result =
                controller.AddAdminField(1, initialViewModel, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<AddAdminFieldViewModel>();
                AssertModelStateErrorIsExpected(result, expectedErrorMessage);
            }
        }

        [Test]
        public void AddAdminField_delete_removes_configured_answer()
        {
            // Given
            var model = new AddAdminFieldViewModel(1, "Test\r\nAnswer");
            const string action = "delete0";
            var initialTempData = new AddAdminFieldData(model);
            controller.TempData.Set(initialTempData);

            // When
            var result = controller.AddAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<AddAdminFieldViewModel>();
                AssertNumberOfConfiguredAnswersOnView(result, 1);
            }
        }

        [Test]
        public void AddAdminField_removes_answer_without_admin_field_selected()
        {
            var model = new AddAdminFieldViewModel(null, "Test\r\nAnswer");
            const string action = "delete0";
            var initialTempData = new AddAdminFieldData(model);
            controller.TempData.Set(initialTempData);

            // When
            var result = controller.AddAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<AddAdminFieldViewModel>();
                AssertNumberOfConfiguredAnswersOnView(result, 1);
            }
        }

        [Test]
        public void AddAdminField_bulk_sets_up_temp_data_and_redirects()
        {
            // Given
            var model = new AddAdminFieldViewModel(1, "Options");
            const string action = "bulk";
            var initialTempData = new AddAdminFieldData(model);
            controller.TempData.Set(initialTempData);

            // When
            var result = controller.AddAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                AssertAddTempDataIsExpected(model);
                result.Should().BeRedirectToActionResult().WithActionName("AddAdminFieldAnswersBulk");
            }
        }

        [Test]
        public void AddAdminField_bulk_redirects_without_admin_field_selected()
        {
            // Given
            var model = new AddAdminFieldViewModel(null, "Options");
            const string action = "bulk";
            var initialTempData = new AddAdminFieldData(model);
            controller.TempData.Set(initialTempData);

            // When
            var result = controller.AddAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                AssertAddTempDataIsExpected(model);
                result.Should().BeRedirectToActionResult().WithActionName("AddAdminFieldAnswersBulk");
            }
        }

        [Test]
        public void AddAdminField_returns_error_with_unexpected_action()
        {
            // Given
            var model = new AddAdminFieldViewModel();
            const string action = "deletetest";
            var initialTempData = new AddAdminFieldData(model);
            controller.TempData.Set(initialTempData);

            // When
            var result = controller.AddAdminField(1, model, action);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void AddAdminFieldAnswersBulk_updates_temp_data_and_redirects_to_add()
        {
            // Given
            var inputViewModel = new AddBulkAdminFieldAnswersViewModel("Test\r\nAnswer", 1);
            var initialAddViewModel = new AddAdminFieldViewModel(1, "Test");
            var expectedViewModel = new AddAdminFieldViewModel(1, "Test\r\nAnswer");
            var initialTempData = new AddAdminFieldData(initialAddViewModel);

            controller.TempData.Set(initialTempData);

            // When
            var result = controller.AddAdminFieldAnswersBulk(1, inputViewModel);

            // Then
            using (new AssertionScope())
            {
                AssertAddTempDataIsExpected(expectedViewModel);
                result.Should().BeRedirectToActionResult().WithActionName("AddAdminField");
            }
        }

        [Test]
        public void RemoveAdminField_removes_admin_field_with_no_user_answers()
        {
            // When
            var result = controller.RemoveAdminField(100, 2);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void RemoveAdminField_returns_remove_view_if_admin_field_has_user_answers()
        {
            // Given
            var removeViewModel = new RemoveAdminFieldViewModel("System Access Granted", 1);

            // When
            var result = controller.RemoveAdminField(100, 1, removeViewModel);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<RemoveAdminFieldViewModel>();
        }

        [Test]
        public void RemoveAdminField_does_not_remove_admin_field_without_confirmation()
        {
            // Given
            var removeViewModel = new RemoveAdminFieldViewModel("System Access Granted", 1);
            removeViewModel.Confirm = false;
            var expectedErrorMessage = "You must confirm before deleting this field";

            // When
            var result = controller.RemoveAdminField(100, 1, removeViewModel);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<RemoveAdminFieldViewModel>();
            controller.ModelState[nameof(RemoveAdminFieldViewModel.Confirm)].Errors[0].ErrorMessage.Should()
                .BeEquivalentTo(expectedErrorMessage);
        }

        [Test]
        public void RemoveAdminField_removes_admin_field_with_confirmation_and_redirects()
        {
            // Given
            var removeViewModel = new RemoveAdminFieldViewModel("System Access Granted", 1);
            removeViewModel.Confirm = true;

            // When
            var result = controller.RemoveAdminField(100, 1, removeViewModel);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        private static void AssertNumberOfConfiguredAnswersOnView(IActionResult result, int expectedCount)
        {
            result.Should().BeViewResult();
            result.As<ViewResult>().Model.As<AdminFieldAnswersViewModel>().Options.Count.Should()
                .Be(expectedCount);
        }

        private void AssertEditTempDataIsExpected(EditAdminFieldViewModel expectedData)
        {
            controller.TempData.Peek<EditAdminFieldData>()!.EditModel.Should()
                .BeEquivalentTo(expectedData);
        }

        private void AssertAddTempDataIsExpected(AddAdminFieldViewModel expectedData)
        {
            controller.TempData.Peek<AddAdminFieldData>()!.AddModel.Should()
                .BeEquivalentTo(expectedData);
        }

        private static void AssertModelStateErrorIsExpected(IActionResult result, string expectedErrorMessage)
        {
            var errorMessage = result.As<ViewResult>().ViewData.ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0).ToList().First().First().ErrorMessage;
            errorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }
    }
}
