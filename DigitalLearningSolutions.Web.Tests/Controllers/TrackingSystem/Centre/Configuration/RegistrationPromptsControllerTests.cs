namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt;
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
        private IMultiPageFormDataService multiPageFormDataService = null!;
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
                    "The complete list of responses must be 4000 characters or fewer (0 characters remaining for the new response, 2 characters were entered)"
                ).SetName("Error_message_shows_zero_characters_remaining_if_options_string_is_at_max_length");
                yield return new TestCaseData(
                    new string('x', 3998),
                    "xx",
                    "The complete list of responses must be 4000 characters or fewer (0 characters remaining for the new response, 2 characters were entered)"
                ).SetName(
                    "Error_message_shows_zero_characters_remaining_if_options_string_is_two_less_than_max_length"
                );
                yield return new TestCaseData(
                    new string('x', 3996),
                    "xxxx",
                    "The complete list of responses must be 4000 characters or fewer (2 characters remaining for the new response, 4 characters were entered)"
                ).SetName("Error_message_shows_two_less_than_number_of_characters_remaining_if_possible_to_add_answer");
            }
        }

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            userDataService = A.Fake<IUserDataService>();
            multiPageFormDataService = A.Fake<IMultiPageFormDataService>();

            registrationPromptsController =
                new RegistrationPromptsController(
                        centreRegistrationPromptsService,
                        userDataService,
                        multiPageFormDataService
                    )
                    .WithDefaultContext()
                    .WithMockUser(true)
                    .WithMockTempData();

            httpRequest = A.Fake<HttpRequest>();
            const string cookieName = "AddRegistrationPromptData";
            const string cookieValue = "AddRegistrationPromptData";

            registrationPromptsControllerWithMockHttpContext =
                new RegistrationPromptsController(
                        centreRegistrationPromptsService,
                        userDataService,
                        multiPageFormDataService
                    )
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
            AssertMultiPageFormDataIsUpdatedCorrectly(
                new AddRegistrationPromptSelectPromptData(),
                new RegistrationPromptAnswersData()
            );
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
            // Given
            var expectedPromptData = new AddRegistrationPromptSelectPromptData(1, true);
            var initialTempData = new AddRegistrationPromptData();
            A.CallTo(
                () => multiPageFormDataService.GetMultiPageFormData<AddRegistrationPromptData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);
            var inputViewModel = new AddRegistrationPromptSelectPromptViewModel(1, true);

            // When
            var result = registrationPromptsController.AddRegistrationPromptSelectPrompt(inputViewModel);

            // Then
            AssertMultiPageFormDataIsUpdatedCorrectly(expectedPromptData, new RegistrationPromptAnswersData());
            result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptConfigureAnswers");
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_next_updates_temp_data()
        {
            // Given
            var expectedPromptData = new AddRegistrationPromptSelectPromptData(1, true);
            var initialTempData = new AddRegistrationPromptData { SelectPromptData = expectedPromptData };
            A.CallTo(
                () => multiPageFormDataService.GetMultiPageFormData<AddRegistrationPromptData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);
            var expectedAnswerData = new RegistrationPromptAnswersData("Test");
            var inputViewModel = new RegistrationPromptAnswersViewModel("Test");
            const string action = "next";

            // When
            var result =
                registrationPromptsController.AddRegistrationPromptConfigureAnswers(inputViewModel, action);

            // Then
            AssertMultiPageFormDataIsUpdatedCorrectly(expectedPromptData, expectedAnswerData);
            result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptSummary");
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_add_configures_new_answer_and_updates_temp_data()
        {
            // Given
            var initialSelectPromptData = new AddRegistrationPromptSelectPromptData(1, true);

            var inputAnswersData = new RegistrationPromptAnswersData("Test", "Answer");
            var expectedAnswersData = new RegistrationPromptAnswersData("Test\r\nAnswer");

            var initialTempData = new AddRegistrationPromptData
                { SelectPromptData = initialSelectPromptData, ConfigureAnswersData = inputAnswersData };
            A.CallTo(
                () => multiPageFormDataService.GetMultiPageFormData<AddRegistrationPromptData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);
            var inputAnswersViewModel = new RegistrationPromptAnswersViewModel("Test", "Answer");

            const string action = "addPrompt";

            // When
            var result =
                registrationPromptsController.AddRegistrationPromptConfigureAnswers(inputAnswersViewModel, action);

            // Then
            using (new AssertionScope())
            {
                AssertMultiPageFormDataIsUpdatedCorrectly(initialSelectPromptData, expectedAnswersData);
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
            var initialSelectPromptData = new AddRegistrationPromptSelectPromptData(1, true);

            var initialAnswersData = new RegistrationPromptAnswersData(optionsString, newAnswerInput);

            var initialTempData = new AddRegistrationPromptData
                { SelectPromptData = initialSelectPromptData, ConfigureAnswersData = initialAnswersData };
            A.CallTo(
                () => multiPageFormDataService.GetMultiPageFormData<AddRegistrationPromptData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);

            const string action = "addPrompt";
            var inputAnswersViewModel = new RegistrationPromptAnswersViewModel(optionsString, newAnswerInput);

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
            var initialPromptData = new AddRegistrationPromptSelectPromptData(1, true);

            var initialAnswersData = new RegistrationPromptAnswersData("Test\r\nAnswer");
            var expectedAnswersData = new RegistrationPromptAnswersData("Answer");

            const string action = "delete0";

            var initialTempData = new AddRegistrationPromptData
                { SelectPromptData = initialPromptData, ConfigureAnswersData = initialAnswersData };
            A.CallTo(
                () => multiPageFormDataService.GetMultiPageFormData<AddRegistrationPromptData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);
            var inputViewModel = new RegistrationPromptAnswersViewModel("Test\r\nAnswer");

            // When
            var result = registrationPromptsController.AddRegistrationPromptConfigureAnswers(inputViewModel, action);

            // Then
            using (new AssertionScope())
            {
                AssertMultiPageFormDataIsUpdatedCorrectly(initialPromptData, expectedAnswersData);
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
            var initialPromptData = new AddRegistrationPromptSelectPromptData(1, true);
            var initialAnswersData = new RegistrationPromptAnswersData("Test\r\nAnswer");
            var initialTempData = new AddRegistrationPromptData
                { SelectPromptData = initialPromptData, ConfigureAnswersData = initialAnswersData };
            A.CallTo(
                () => multiPageFormDataService.GetMultiPageFormData<AddRegistrationPromptData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);
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
                A.CallTo(
                    () => multiPageFormDataService.ClearMultiPageFormData(
                        MultiPageFormDataFeature.AddRegistrationPrompt,
                        registrationPromptsController.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
        }

        [Test]
        public void AddRegistrationPromptSummary_calls_registration_prompt_service_and_redirects_to_error_on_failure()
        {
            // Given
            var initialPromptData = new AddRegistrationPromptSelectPromptData(1, true);
            var initialAnswersData = new RegistrationPromptAnswersData("Test\r\nAnswer");
            var initialTempData = new AddRegistrationPromptData
                { SelectPromptData = initialPromptData, ConfigureAnswersData = initialAnswersData };
            A.CallTo(
                () => multiPageFormDataService.GetMultiPageFormData<AddRegistrationPromptData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);
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
            var initialPromptData = new AddRegistrationPromptSelectPromptData(1, true);
            var initialAnswersData = new RegistrationPromptAnswersData("Test");
            var expectedAnswersData = new RegistrationPromptAnswersData("Test\r\nAnswer");

            var initialTempData = new AddRegistrationPromptData
            {
                SelectPromptData = initialPromptData,
                ConfigureAnswersData = initialAnswersData,
            };
            A.CallTo(
                () => multiPageFormDataService.GetMultiPageFormData<AddRegistrationPromptData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);

            // When
            var result = registrationPromptsController.AddRegistrationPromptBulkPost(inputViewModel);

            // Then
            using (new AssertionScope())
            {
                AssertMultiPageFormDataIsUpdatedCorrectly(initialPromptData, expectedAnswersData);
                result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptConfigureAnswers");
            }
        }

        [Test]
        public void
            EditRegistrationPromptBulkPost_adds_model_error_if_trimmed_case_insensitive_bulk_edit_is_not_unique()
        {
            // Given
            var model = new BulkRegistrationPromptAnswersViewModel("test\r\n   tEsT   ", false, 1);

            // When
            var result = registrationPromptsController.EditRegistrationPromptBulkPost(model);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<BulkRegistrationPromptAnswersViewModel>();
                AssertModelStateErrorIsExpected(result, "The list of responses contains duplicate options");
            }
        }

        private void AssertSelectPromptViewModelIsExpectedModel(AddRegistrationPromptSelectPromptData promptModel)
        {
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.SelectPromptData.Should()
                .BeEquivalentTo(promptModel);
        }

        private void AssertPromptAnswersViewModelIsExpectedModel(RegistrationPromptAnswersViewModel promptModel)
        {
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.ConfigureAnswersData.Should()
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

        private void AssertMultiPageFormDataIsUpdatedCorrectly(
            AddRegistrationPromptSelectPromptData expectedPromptData,
            RegistrationPromptAnswersData expectedAnswersData
        )
        {
            A.CallTo(
                () => multiPageFormDataService.SetMultiPageFormData(
                    A<AddRegistrationPromptData>.That.Matches(
                        d => d.SelectPromptData.Mandatory == expectedPromptData.Mandatory &&
                             d.SelectPromptData.CustomPromptId == expectedPromptData.CustomPromptId &&
                             d.ConfigureAnswersData.OptionsString == expectedAnswersData.OptionsString &&
                             d.ConfigureAnswersData.Answer == expectedAnswersData.Answer &&
                             d.ConfigureAnswersData.IncludeAnswersTableCaption ==
                             expectedAnswersData.IncludeAnswersTableCaption
                    ),
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).MustHaveHappenedOnceExactly();
        }
    }
}
