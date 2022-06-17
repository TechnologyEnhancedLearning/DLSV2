namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddAdminField;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditAdminField;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Extensions;
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
        private AdminFieldsController controller = null!;
        private ICourseAdminFieldsDataService courseAdminFieldsDataService = null!;
        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ICourseService courseService = null!;
        private IMultiPageFormService multiPageFormService = null!;

        private static IEnumerable<TestCaseData> AddAnswerModelErrorTestData
        {
            get
            {
                yield return new TestCaseData(
                    new string('x', 1000),
                    "xx",
                    "The complete list of responses must be 1000 characters or fewer (0 characters remaining for the new response, 2 characters were entered)"
                ).SetName("Error_message_shows_zero_characters_remaining_if_options_string_is_at_max_length");
                yield return new TestCaseData(
                    new string('x', 998),
                    "xx",
                    "The complete list of responses must be 1000 characters or fewer (0 characters remaining for the new response, 2 characters were entered)"
                ).SetName(
                    "Error_message_shows_zero_characters_remaining_if_options_string_is_two_less_than_max_length"
                );
                yield return new TestCaseData(
                    new string('x', 996),
                    "xxxx",
                    "The complete list of responses must be 1000 characters or fewer (2 characters remaining for the new response, 4 characters were entered)"
                ).SetName("Error_message_shows_two_less_than_number_of_characters_remaining_if_possible_to_add_answer");
            }
        }

        [SetUp]
        public void Setup()
        {
            courseAdminFieldsDataService = A.Fake<ICourseAdminFieldsDataService>();
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
            multiPageFormService = A.Fake<IMultiPageFormService>();
            courseService = A.Fake<ICourseService>();
            controller = new AdminFieldsController(
                    courseAdminFieldsService,
                    courseAdminFieldsDataService,
                    multiPageFormService
                )
                .WithDefaultContext()
                .WithMockUser(true, 101)
                .WithMockTempData();
        }

        [Test]
        public void AdminFields_returns_AdminFields_page_when_appropriate_course_found_and_clears_TempData()
        {
            // Given
            var courseAdminField1 =
                PromptsTestHelper.GetDefaultCourseAdminField(1, "System Access Granted", "Yes\r\nNo");
            var courseAdminFields = new List<CourseAdminField> { courseAdminField1 };
            A.CallTo(() => courseAdminFieldsService.GetCourseAdminFieldsForCourse(A<int>._))
                .Returns(PromptsTestHelper.GetDefaultCourseAdminFields(courseAdminFields));
            controller.TempData.Set(courseAdminField1);

            // When
            var result = controller.Index(1);

            // Then
            controller.TempData.Peek<CourseAdminField>().Should().BeNull();
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<AdminFieldsViewModel>();
        }

        [Test]
        public void PostEditAdminField_save_calls_correct_methods()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, "Test", "Options");
            const string action = "save";

            A.CallTo(
                () => courseAdminFieldsService.UpdateAdminFieldForCourse(
                    1,
                    1,
                    "Options"
                )
            ).DoesNothing();

            // When
            var result = controller.EditAdminField(1, model, action);

            // Then
            A.CallTo(
                () => courseAdminFieldsService.UpdateAdminFieldForCourse(
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
            var model = new EditAdminFieldViewModel(1, "Test", "Options")
            {
                Answer = "new option",
            };
            const string action = "addPrompt";

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
            var expectedData = new EditAdminFieldTempData
            {
                PromptNumber = model.PromptNumber,
                Prompt = model.Prompt,
                OptionsString = model.OptionsString,
                Answer = model.Answer,
                IncludeAnswersTableCaption = model.IncludeAnswersTableCaption,
            };
            const string action = "bulk";

            // When
            var result = controller.EditAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                AssertEditAdminFieldMultiPageFormDataIsUpdatedCorrectly(expectedData);
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
            var initialTempData = new EditAdminFieldTempData
            {
                PromptNumber = initialEditViewModel.PromptNumber,
                Prompt = initialEditViewModel.Prompt,
                OptionsString = initialEditViewModel.OptionsString,
                Answer = initialEditViewModel.Answer,
                IncludeAnswersTableCaption = initialEditViewModel.IncludeAnswersTableCaption,
            };

            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<EditAdminFieldTempData>(
                    MultiPageFormDataFeature.EditAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

            A.CallTo(() => courseService.VerifyAdminUserCanManageCourse(A<int>._, A<int>._, A<int>._))
                .Returns(true);

            // When
            var result = controller.EditAdminFieldAnswersBulk(1, 1, inputViewModel);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormService.SetMultiPageFormData(
                        A<EditAdminFieldTempData>.That.Matches(
                            d => d.PromptNumber == initialTempData.PromptNumber &&
                                 d.Prompt == initialTempData.Prompt &&
                                 d.Answer == initialTempData.Answer &&
                                 d.IncludeAnswersTableCaption == initialTempData.IncludeAnswersTableCaption &&
                                 d.OptionsString == inputViewModel.OptionsString
                        ),
                        MultiPageFormDataFeature.EditAdminField,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("EditAdminField");
            }
        }

        [Test]
        public void AddAdminFieldNew_sets_new_temp_data()
        {
            // When
            var result = controller.AddAdminFieldNew(1);

            // Then
            AssertAddAdminFieldMultiPageFormDataIsUpdatedCorrectly(new AddAdminFieldTempData());
            result.Should().BeRedirectToActionResult().WithActionName("AddAdminField");
        }

        [Test]
        public void AddAdminField_save_redirects_to_index()
        {
            // Given
            var model = new AddAdminFieldViewModel(1, "Test");
            const string action = "save";
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = model.AdminFieldId, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

            A.CallTo(
                () => courseAdminFieldsService.AddAdminFieldToCourse(
                    100,
                    1,
                    "Test"
                )
            ).Returns(true);

            // When
            var result = controller.AddAdminField(100, model, action);

            // Then
            A.CallTo(
                () => multiPageFormService.ClearMultiPageFormData(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void AddAdminField_save_redirects_successfully_without_answers_configured()
        {
            // Given
            var model = new AddAdminFieldViewModel(1, null);
            const string action = "save";
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = model.AdminFieldId, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

            A.CallTo(
                () => courseAdminFieldsService.AddAdminFieldToCourse(
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
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = model.AdminFieldId, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

            A.CallTo(
                () => courseAdminFieldsService.AddAdminFieldToCourse(
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
            var initialTempData = new AddAdminFieldTempData
            {
                AdminFieldId = initialViewModel.AdminFieldId,
                OptionsString = initialViewModel.OptionsString,
                Answer = initialViewModel.Answer,
                IncludeAnswersTableCaption = initialViewModel.IncludeAnswersTableCaption,
            };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

            var expectedData = new AddAdminFieldTempData
            {
                AdminFieldId = 1,
                OptionsString = "Test\r\nAnswer",
                Answer = null,
                IncludeAnswersTableCaption = initialViewModel.IncludeAnswersTableCaption,
            };
            const string action = "addPrompt";

            // When
            var result =
                controller.AddAdminField(1, initialViewModel, action);

            // Then
            using (new AssertionScope())
            {
                AssertAddAdminFieldMultiPageFormDataIsUpdatedCorrectly(expectedData);
                result.As<ViewResult>().Model.Should().BeOfType<AddAdminFieldViewModel>();
                AssertNumberOfConfiguredAnswersOnView(result, 2);
            }
        }

        [Test]
        public void AddAdminField_adds_answer_without_admin_field_selected()
        {
            var initialViewModel = new AddAdminFieldViewModel(null, null, "Answer");
            var initialTempData = new AddAdminFieldTempData
            {
                AdminFieldId = initialViewModel.AdminFieldId,
                OptionsString = initialViewModel.OptionsString,
                Answer = initialViewModel.Answer,
                IncludeAnswersTableCaption = initialViewModel.IncludeAnswersTableCaption,
            };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

            const string action = "addPrompt";

            // When
            controller.AddAdminField(1, initialViewModel, action);

            // Then
            AssertAddAdminFieldMultiPageFormDataIsUpdatedCorrectly(initialTempData);
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
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = initialViewModel.AdminFieldId, OptionsString = initialViewModel.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);
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
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = model.AdminFieldId, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

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
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = model.AdminFieldId, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

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
            var initialTempData = new AddAdminFieldTempData
            {
                AdminFieldId = model.AdminFieldId,
                OptionsString = model.OptionsString,
                Answer = model.Answer,
                IncludeAnswersTableCaption = model.IncludeAnswersTableCaption,
            };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

            // When
            var result = controller.AddAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                AssertAddAdminFieldMultiPageFormDataIsUpdatedCorrectly(initialTempData);
                result.Should().BeRedirectToActionResult().WithActionName("AddAdminFieldAnswersBulk");
            }
        }

        [Test]
        public void AddAdminField_bulk_redirects_without_admin_field_selected()
        {
            // Given
            var model = new AddAdminFieldViewModel(null, "Options");
            const string action = "bulk";
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = model.AdminFieldId, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

            // When
            var result = controller.AddAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                AssertAddAdminFieldMultiPageFormDataIsUpdatedCorrectly(initialTempData);
                result.Should().BeRedirectToActionResult().WithActionName("AddAdminFieldAnswersBulk");
            }
        }

        [Test]
        public void AddAdminField_returns_error_with_unexpected_action()
        {
            // Given
            var model = new AddAdminFieldViewModel();
            const string action = "deletetest";
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = model.AdminFieldId, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

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
            var expectedData = new AddAdminFieldTempData { AdminFieldId = 1, OptionsString = "Test\r\nAnswer" };
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = initialAddViewModel.AdminFieldId, OptionsString = initialAddViewModel.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);

            // When
            var result = controller.AddAdminFieldAnswersBulk(1, inputViewModel);

            // Then
            using (new AssertionScope())
            {
                AssertAddAdminFieldMultiPageFormDataIsUpdatedCorrectly(expectedData);
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

        [Test]
        public void AddAdminField_adds_model_error_if_field_name_is_already_in_use()
        {
            // Given
            var model = new AddAdminFieldViewModel(1, "test");
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = model.AdminFieldId, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);
            const string action = "save";

            A.CallTo(() => courseAdminFieldsDataService.GetCourseFieldPromptIdsForCustomisation(A<int>._))
                .Returns(new[] { 1, 0, 0 });

            // When
            var result = controller.AddAdminField(100, model, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<AddAdminFieldViewModel>();
                AssertModelStateErrorIsExpected(result, "That field name already exists for this course");
            }
        }

        [Test]
        public void AddAdminField_adds_model_error_if_trimmed_case_insensitive_answer_is_already_in_options_list()
        {
            // Given
            var model = new AddAdminFieldViewModel(1, "test", "  tEsT  ");
            var initialTempData = new AddAdminFieldTempData
                { AdminFieldId = model.AdminFieldId, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);
            const string action = "addPrompt";

            // When
            var result = controller.AddAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<AddAdminFieldViewModel>();
                AssertModelStateErrorIsExpected(result, "That response is already in the list of options");
            }
        }

        [Test]
        public void EditAdminField_adds_model_error_if_trimmed_case_insensitive_answer_is_already_in_options_list()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, "prompt", "test");
            model.Answer = "  tEsT  ";
            var initialTempData = new EditAdminFieldTempData
                { PromptNumber = model.PromptNumber, Prompt = model.Prompt, OptionsString = model.OptionsString };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<EditAdminFieldTempData>(
                    MultiPageFormDataFeature.EditAdminField,
                    controller.TempData
                )
            ).Returns(initialTempData);
            const string action = "addPrompt";

            // When
            var result = controller.EditAdminField(1, model, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditAdminFieldViewModel>();
                AssertModelStateErrorIsExpected(result, "That response is already in the list of options");
            }
        }

        [Test]
        public void AddAdminFieldAnswersBulk_adds_model_error_if_trimmed_case_insensitive_bulk_edit_is_not_unique()
        {
            // Given
            var model = new AddBulkAdminFieldAnswersViewModel("test\r\n   tEsT   ");

            // When
            var result = controller.AddAdminFieldAnswersBulk(1, model);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<AddBulkAdminFieldAnswersViewModel>();
                AssertModelStateErrorIsExpected(result, "The list of responses contains duplicate options");
            }
        }

        [Test]
        public void EditAdminFieldAnswersBulk_adds_model_error_if_trimmed_case_insensitive_bulk_edit_is_not_unique()
        {
            // Given
            var model = new BulkAdminFieldAnswersViewModel("test\r\n   tEsT   ");

            // When
            var result = controller.EditAdminFieldAnswersBulk(1, 1, model);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<BulkAdminFieldAnswersViewModel>();
                AssertModelStateErrorIsExpected(result, "The list of responses contains duplicate options");
            }
        }

        private static void AssertNumberOfConfiguredAnswersOnView(IActionResult result, int expectedCount)
        {
            result.Should().BeViewResult();
            result.As<ViewResult>().Model.As<AdminFieldAnswersViewModel>().Options.Count.Should()
                .Be(expectedCount);
        }

        private static void AssertModelStateErrorIsExpected(IActionResult result, string expectedErrorMessage)
        {
            var errorMessage = result.As<ViewResult>().ViewData.ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0).ToList().First().First().ErrorMessage;
            errorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }

        private void AssertAddAdminFieldMultiPageFormDataIsUpdatedCorrectly(
            AddAdminFieldTempData expectedTempData
        )
        {
            A.CallTo(
                () => multiPageFormService.SetMultiPageFormData(
                    A<AddAdminFieldTempData>.That.Matches(
                        d => d.AdminFieldId == expectedTempData.AdminFieldId &&
                             d.OptionsString == expectedTempData.OptionsString &&
                             d.Answer == expectedTempData.Answer &&
                             d.IncludeAnswersTableCaption ==
                             expectedTempData.IncludeAnswersTableCaption
                    ),
                    MultiPageFormDataFeature.AddAdminField,
                    controller.TempData
                )
            ).MustHaveHappened();
        }

        private void AssertEditAdminFieldMultiPageFormDataIsUpdatedCorrectly(
            EditAdminFieldTempData expectedTempData
        )
        {
            A.CallTo(
                () => multiPageFormService.SetMultiPageFormData(
                    A<EditAdminFieldTempData>.That.Matches(
                        d => d.PromptNumber == expectedTempData.PromptNumber &&
                             d.Prompt == expectedTempData.Prompt &&
                             d.OptionsString == expectedTempData.OptionsString &&
                             d.Answer == expectedTempData.Answer &&
                             d.IncludeAnswersTableCaption ==
                             expectedTempData.IncludeAnswersTableCaption
                    ),
                    MultiPageFormDataFeature.EditAdminField,
                    controller.TempData
                )
            ).MustHaveHappened();
        }
    }
}
