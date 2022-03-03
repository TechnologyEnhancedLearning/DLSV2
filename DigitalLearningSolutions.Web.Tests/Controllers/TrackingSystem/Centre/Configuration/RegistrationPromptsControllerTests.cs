namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Configuration;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class RegistrationPromptsControllerTests
    {
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private HttpRequest httpRequest = null!;
        private RegistrationPromptsController registrationPromptsController = null!;
        private RegistrationPromptsController registrationPromptsControllerWithMockHttpContext = null!;
        private IUserDataService userDataService = null!;

        private static IEnumerable<TestCaseData> AddAnswerModelErrorTestData
        {
            get
            {
                yield return new TestCaseData(
                    new string('x', 4000),
                    "xx",
                    "The complete list of answers must be 4000 characters or fewer (0 characters remaining for the new answer, 2 characters were entered)"
                ).SetName("Error_message_shows_zero_characters_remaining_if_options_string_is_at_max_length");
                yield return new TestCaseData(
                    new string('x', 3998),
                    "xx",
                    "The complete list of answers must be 4000 characters or fewer (0 characters remaining for the new answer, 2 characters were entered)"
                ).SetName(
                    "Error_message_shows_zero_characters_remaining_if_options_string_is_two_less_than_max_length"
                );
                yield return new TestCaseData(
                    new string('x', 3996),
                    "xxxx",
                    "The complete list of answers must be 4000 characters or fewer (2 characters remaining for the new answer, 4 characters were entered)"
                ).SetName("Error_message_shows_two_less_than_number_of_characters_remaining_if_possible_to_add_answer");
            }
        }

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            userDataService = A.Fake<IUserDataService>();

            registrationPromptsController =
                new RegistrationPromptsController(centreRegistrationPromptsService, userDataService)
                    .WithDefaultContext()
                    .WithMockUser(true)
                    .WithMockTempData();

            httpRequest = A.Fake<HttpRequest>();
            const string cookieName = "AddRegistrationPromptData";
            const string cookieValue = "AddRegistrationPromptData";

            registrationPromptsControllerWithMockHttpContext =
                new RegistrationPromptsController(centreRegistrationPromptsService, userDataService)
                    .WithMockHttpContext(httpRequest, cookieName, cookieValue)
                    .WithMockUser(true)
                    .WithMockTempData();
        }

        [Test]
        public void Index_should_clear_TempData_and_go_to_index_page()
        {
            var expectedTempData = new AddRegistrationPromptData();
            registrationPromptsController.TempData.Set(expectedTempData);

            // When
            var result = registrationPromptsController.Index();

            // Then
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>().Should().BeNull();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PostEditRegistrationPrompt_save_calls_correct_methods()
        {
            // Given
            var model = new EditRegistrationPromptViewModel(1, "Test", false, "Test");
            const string action = "save";

            A.CallTo(
                () => centreRegistrationPromptsService.UpdateCentreRegistrationPrompt(
                    ControllerContextHelper.CentreId,
                    1,
                    false,
                    "Test"
                )
            ).DoesNothing();

            // When
            var result = registrationPromptsController.EditRegistrationPrompt(model, action);

            // Then
            A.CallTo(
                () => centreRegistrationPromptsService.UpdateCentreRegistrationPrompt(
                    ControllerContextHelper.CentreId,
                    1,
                    false,
                    "Test"
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void PostEditRegistrationPrompt_add_configures_new_answer()
        {
            // Given
            var model = new EditRegistrationPromptViewModel(1, "Test", false, "Test", "Answer");
            const string action = "addPrompt";

            A.CallTo(
                () => centreRegistrationPromptsService.UpdateCentreRegistrationPrompt(
                    ControllerContextHelper.CentreId,
                    1,
                    false,
                    "Test"
                )
            ).DoesNothing();

            // When
            var result = registrationPromptsController.EditRegistrationPrompt(model, action);

            // Then
            result.As<ViewResult>().Model.Should().BeOfType<EditRegistrationPromptViewModel>().Which.OptionsString
                .Should().BeEquivalentTo("Test\r\nAnswer");
        }

        [Test]
        public void PostEditRegistrationPrompt_delete_removes_configured_answer()
        {
            // Given
            var model = new EditRegistrationPromptViewModel(1, "Test", false, "Test\r\nAnswer", "Answer");
            const string action = "delete0";

            // When
            var result = registrationPromptsController.EditRegistrationPrompt(model, action);

            // Then
            result.As<ViewResult>().Model.Should().BeOfType<EditRegistrationPromptViewModel>().Which.OptionsString
                .Should().BeEquivalentTo("Answer");
        }

        [Test]
        public void PostEditRegistrationPrompt_bulk_sets_up_temp_data_and_redirects()
        {
            // Given
            var model = new EditRegistrationPromptViewModel(1, "Test", false, "Test\r\nAnswer", "Answer");
            const string action = "bulk";

            // When
            var result = registrationPromptsController.EditRegistrationPrompt(model, action);

            // Then
            using (new AssertionScope())
            {
                AssertEditTempDataIsExpected(model);
                result.Should().BeRedirectToActionResult().WithActionName("EditRegistrationPromptBulk");
            }
        }

        [Test]
        public void PostEditRegistrationPrompt_returns_error_with_unexpected_action()
        {
            // Given
            var model = new EditRegistrationPromptViewModel(1, "Test", false, "Test\r\nAnswer", "Answer");
            const string action = "deletetest";

            // When
            var result = registrationPromptsController.EditRegistrationPrompt(model, action);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void AddRegistrationPromptNew_sets_new_temp_data()
        {
            // When
            var result = registrationPromptsController.AddRegistrationPromptNew();

            // Then
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>().Should()
                .NotBeNull();
            result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptSelectPrompt");
        }

        [Test]
        public void AddRegistrationPromptSelectPrompt_loads_existing_temp_data()
        {
            var expectedTempData = new AddRegistrationPromptData();
            registrationPromptsControllerWithMockHttpContext.TempData.Set(expectedTempData);

            // When
            var result = registrationPromptsControllerWithMockHttpContext.AddRegistrationPromptSelectPrompt();

            // Then
            registrationPromptsControllerWithMockHttpContext.TempData.Peek<AddRegistrationPromptData>().Should()
                .BeEquivalentTo(expectedTempData);
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void AddRegistrationPromptSelectPrompt_post_updates_temp_data_and_redirects()
        {
            var expectedPromptModel = new AddRegistrationPromptSelectPromptViewModel(1, true);
            var initialTempData = new AddRegistrationPromptData();
            registrationPromptsController.TempData.Set(initialTempData);

            // When
            var result = registrationPromptsController.AddRegistrationPromptSelectPrompt(expectedPromptModel);

            // Then
            AssertSelectPromptViewModelIsExpectedModel(expectedPromptModel);
            result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptConfigureAnswers");
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_next_updates_temp_data()
        {
            // Given
            var expectedPromptModel = new AddRegistrationPromptSelectPromptViewModel(1, true);
            var initialTempData = new AddRegistrationPromptData { SelectPromptViewModel = expectedPromptModel };
            registrationPromptsController.TempData.Set(initialTempData);
            var expectedAnswerModel = new RegistrationPromptAnswersViewModel("Test");
            const string action = "next";

            // When
            var result =
                registrationPromptsController.AddRegistrationPromptConfigureAnswers(expectedAnswerModel, action);

            // Then
            AssertSelectPromptViewModelIsExpectedModel(expectedPromptModel);
            AssertPromptAnswersViewModelIsExpectedModel(expectedAnswerModel);
            result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptSummary");
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_add_configures_new_answer_and_updates_temp_data()
        {
            // Given
            var initialSelectPromptModel = new AddRegistrationPromptSelectPromptViewModel(1, true);

            var inputAnswersViewModel = new RegistrationPromptAnswersViewModel("Test", "Answer");
            var expectedConfigureAnswerViewModel = new RegistrationPromptAnswersViewModel("Test\r\nAnswer");

            var initialTempData = new AddRegistrationPromptData
                { SelectPromptViewModel = initialSelectPromptModel, ConfigureAnswersViewModel = inputAnswersViewModel };
            registrationPromptsController.TempData.Set(initialTempData);

            const string action = "addPrompt";

            // When
            var result =
                registrationPromptsController.AddRegistrationPromptConfigureAnswers(inputAnswersViewModel, action);

            // Then
            using (new AssertionScope())
            {
                AssertSelectPromptViewModelIsExpectedModel(initialSelectPromptModel);
                AssertPromptAnswersViewModelIsExpectedModel(expectedConfigureAnswerViewModel);
                result.As<ViewResult>().Model.Should().BeOfType<RegistrationPromptAnswersViewModel>();
            }
        }

        [Test]
        [TestCaseSource(
            typeof(RegistrationPromptsControllerTests),
            nameof(AddAnswerModelErrorTestData)
        )]
        public void
            AddRegistrationPromptConfigureAnswers_adds_correct_model_error_if_new_answer_surpasses_character_limit(
                string optionsString,
                string newAnswerInput,
                string expectedErrorMessage
            )
        {
            // Given
            var initialSelectPromptModel = new AddRegistrationPromptSelectPromptViewModel(1, true);

            var inputAnswersViewModel = new RegistrationPromptAnswersViewModel(optionsString, newAnswerInput);

            var initialTempData = new AddRegistrationPromptData
                { SelectPromptViewModel = initialSelectPromptModel, ConfigureAnswersViewModel = inputAnswersViewModel };
            registrationPromptsController.TempData.Set(initialTempData);

            const string action = "addPrompt";

            // When
            var result =
                registrationPromptsController.AddRegistrationPromptConfigureAnswers(inputAnswersViewModel, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<RegistrationPromptAnswersViewModel>();
                AssertModelStateErrorIsExpected(result, expectedErrorMessage);
            }
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_delete_removes_configured_answer()
        {
            // Given
            var initialPromptModel = new AddRegistrationPromptSelectPromptViewModel(1, true);

            var initialViewModel = new RegistrationPromptAnswersViewModel("Test\r\nAnswer");
            var expectedViewModel = new RegistrationPromptAnswersViewModel("Answer");

            const string action = "delete0";

            var initialTempData = new AddRegistrationPromptData
                { SelectPromptViewModel = initialPromptModel, ConfigureAnswersViewModel = initialViewModel };
            registrationPromptsController.TempData.Set(initialTempData);

            // When
            var result = registrationPromptsController.AddRegistrationPromptConfigureAnswers(initialViewModel, action);

            // Then
            using (new AssertionScope())
            {
                AssertSelectPromptViewModelIsExpectedModel(initialPromptModel);
                AssertPromptAnswersViewModelIsExpectedModel(expectedViewModel);
                result.As<ViewResult>().Model.Should().BeOfType<RegistrationPromptAnswersViewModel>();
            }
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_returns_error_with_unexpected_action()
        {
            // Given
            var model = new RegistrationPromptAnswersViewModel("Test\r\nAnswer", "Answer");
            const string action = "deletetest";

            // When
            var result = registrationPromptsController.AddRegistrationPromptConfigureAnswers(model, action);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void AddRegistrationPromptSummary_calls_registration_prompt_service_and_redirects_to_index()
        {
            // Given
            var initialPromptModel = new AddRegistrationPromptSelectPromptViewModel(1, true);
            var initialViewModel = new RegistrationPromptAnswersViewModel("Test\r\nAnswer");
            var initialTempData = new AddRegistrationPromptData
                { SelectPromptViewModel = initialPromptModel, ConfigureAnswersViewModel = initialViewModel };
            registrationPromptsController.TempData.Set(initialTempData);
            A.CallTo(
                () => centreRegistrationPromptsService.AddCentreRegistrationPrompt(
                    ControllerContextHelper.CentreId,
                    1,
                    true,
                    "Test\r\nAnswer"
                )
            ).Returns(true);

            // When
            var result = registrationPromptsController.AddRegistrationPromptSummaryPost();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => centreRegistrationPromptsService.AddCentreRegistrationPrompt(
                        ControllerContextHelper.CentreId,
                        1,
                        true,
                        "Test\r\nAnswer"
                    )
                ).MustHaveHappened();
                registrationPromptsController.TempData.Peek<AddRegistrationPromptData>().Should().BeNull();
                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
        }

        [Test]
        public void AddRegistrationPromptSummary_calls_registration_prompt_service_and_redirects_to_error_on_failure()
        {
            // Given
            var initialPromptModel = new AddRegistrationPromptSelectPromptViewModel(1, true);
            var initialViewModel = new RegistrationPromptAnswersViewModel("Test\r\nAnswer");
            var initialTempData = new AddRegistrationPromptData
                { SelectPromptViewModel = initialPromptModel, ConfigureAnswersViewModel = initialViewModel };
            registrationPromptsController.TempData.Set(initialTempData);
            A.CallTo(
                () => centreRegistrationPromptsService.AddCentreRegistrationPrompt(
                    ControllerContextHelper.CentreId,
                    1,
                    true,
                    "Test\r\nAnswer"
                )
            ).Returns(false);

            // When
            var result = registrationPromptsController.AddRegistrationPromptSummaryPost();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => centreRegistrationPromptsService.AddCentreRegistrationPrompt(
                        ControllerContextHelper.CentreId,
                        1,
                        true,
                        "Test\r\nAnswer"
                    )
                ).MustHaveHappened();
                result.Should().BeStatusCodeResult().WithStatusCode(500);
            }
        }

        [Test]
        public void RegistrationPromptBulkPost_updates_temp_data_and_redirects_to_edit()
        {
            // Given
            var inputViewModel = new BulkRegistrationPromptAnswersViewModel("Test\r\nAnswer", false, 1);
            var initialEditViewModel = new EditRegistrationPromptViewModel(1, "Prompt", false, "Test");
            var expectedViewModel = new EditRegistrationPromptViewModel(1, "Prompt", false, "Test\r\nAnswer");
            var initialTempData = new EditRegistrationPromptData(initialEditViewModel);

            registrationPromptsController.TempData.Set(initialTempData);

            // When
            var result = registrationPromptsController.EditRegistrationPromptBulkPost(inputViewModel);

            // Then
            using (new AssertionScope())
            {
                AssertEditTempDataIsExpected(expectedViewModel);
                result.Should().BeRedirectToActionResult().WithActionName("EditRegistrationPrompt");
            }
        }

        [Test]
        public void RegistrationPromptBulkPost_updates_temp_data_and_redirects_to_configure_answers()
        {
            // Given
            var inputViewModel = new BulkRegistrationPromptAnswersViewModel("Test\r\nAnswer", true, null);
            var initialPromptModel = new AddRegistrationPromptSelectPromptViewModel(1, true);
            var initialConfigureAnswersViewModel = new RegistrationPromptAnswersViewModel("Test");
            var expectedViewModel = new RegistrationPromptAnswersViewModel("Test\r\nAnswer");

            var initialTempData = new AddRegistrationPromptData
            {
                SelectPromptViewModel = initialPromptModel,
                ConfigureAnswersViewModel = initialConfigureAnswersViewModel,
            };
            registrationPromptsController.TempData.Set(initialTempData);

            // When
            var result = registrationPromptsController.AddRegistrationPromptBulkPost(inputViewModel);

            // Then
            using (new AssertionScope())
            {
                AssertSelectPromptViewModelIsExpectedModel(initialPromptModel);
                AssertPromptAnswersViewModelIsExpectedModel(expectedViewModel);
                result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptConfigureAnswers");
            }
        }

        private void AssertSelectPromptViewModelIsExpectedModel(AddRegistrationPromptSelectPromptViewModel promptModel)
        {
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.SelectPromptViewModel.Should()
                .BeEquivalentTo(promptModel);
        }

        private void AssertPromptAnswersViewModelIsExpectedModel(RegistrationPromptAnswersViewModel promptModel)
        {
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.ConfigureAnswersViewModel.Should()
                .BeEquivalentTo(promptModel);
        }

        private void AssertEditTempDataIsExpected(EditRegistrationPromptViewModel expectedData)
        {
            registrationPromptsController.TempData.Peek<EditRegistrationPromptData>()!.EditModel.Should()
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
