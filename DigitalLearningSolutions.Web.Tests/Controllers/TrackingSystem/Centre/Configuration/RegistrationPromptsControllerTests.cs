namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditRegistrationPrompt;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Configuration;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;

  public class RegistrationPromptsControllerTests
    {
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private HttpRequest httpRequest = null!;
        private IMultiPageFormService multiPageFormService = null!;
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
            multiPageFormService = A.Fake<IMultiPageFormService>();

            registrationPromptsController =
                new RegistrationPromptsController(
                        centreRegistrationPromptsService,
                        userDataService,
                        multiPageFormService
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
                        multiPageFormService
                    )
                    .WithMockHttpContext(httpRequest, cookieName, cookieValue)
                    .WithMockUser(true)
                    .WithMockTempData();
        }

        [Test]
        public void Index_should_clear_TempData_and_go_to_index_page()
        {
            var expectedTempData = new AddRegistrationPromptTempData();
            registrationPromptsController.TempData.Set(expectedTempData);

            // When
            var result = registrationPromptsController.Index();

            // Then
            registrationPromptsController.TempData.Peek<AddRegistrationPromptTempData>().Should().BeNull();
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
                A.CallTo(
                    () => multiPageFormService.SetMultiPageFormData(
                        A<EditRegistrationPromptTempData>.That.Matches(
                            d => d.PromptNumber == model.PromptNumber &&
                                 d.Prompt == model.Prompt &&
                                 d.Mandatory == model.Mandatory &&
                                 d.Answer == model.Answer &&
                                 d.IncludeAnswersTableCaption == model.IncludeAnswersTableCaption &&
                                 d.OptionsString == model.OptionsString
                        ),
                        MultiPageFormDataFeature.EditRegistrationPrompt,
                        registrationPromptsController.TempData
                    )
                ).MustHaveHappenedOnceExactly();
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
                new RegistrationPromptAnswersTempData()
            );
            result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptSelectPrompt");
        }

        [Test]
        public void AddRegistrationPromptSelectPrompt_loads_existing_temp_data()
        {
            var expectedTempData = new AddRegistrationPromptTempData();
            registrationPromptsControllerWithMockHttpContext.TempData.Set(expectedTempData);

            // When
            var result = registrationPromptsControllerWithMockHttpContext.AddRegistrationPromptSelectPrompt();

            // Then
            registrationPromptsControllerWithMockHttpContext.TempData.Peek<AddRegistrationPromptTempData>().Should()
                .BeEquivalentTo(expectedTempData);
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void AddRegistrationPromptSelectPrompt_post_updates_temp_data_and_redirects()
        {
            // Given
            var expectedPromptData = new AddRegistrationPromptSelectPromptData(1, true, "prompt");
            var initialTempData = new AddRegistrationPromptTempData();
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsAlphabeticalList())
                .Returns(new List<(int id, string value)> { (1, "prompt") });
            var inputViewModel = new AddRegistrationPromptSelectPromptViewModel(1, true);

            // When
            var result = registrationPromptsController.AddRegistrationPromptSelectPrompt(inputViewModel);

            // Then
            AssertMultiPageFormDataIsUpdatedCorrectly(expectedPromptData, new RegistrationPromptAnswersTempData());
            result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptConfigureAnswers");
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_next_updates_temp_data()
        {
            // Given
            var expectedPromptData = new AddRegistrationPromptSelectPromptData(1, true, "prompt");
            var initialTempData = new AddRegistrationPromptTempData { SelectPromptData = expectedPromptData };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);
            var expectedAnswerData = new RegistrationPromptAnswersTempData("Test");
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
            var initialSelectPromptData = new AddRegistrationPromptSelectPromptData(1, true, "prompt");

            var inputAnswersData = new RegistrationPromptAnswersTempData("Test", "Answer");
            var expectedAnswersData = new RegistrationPromptAnswersTempData("Test\r\nAnswer");

            var initialTempData = new AddRegistrationPromptTempData
            { SelectPromptData = initialSelectPromptData, ConfigureAnswersTempData = inputAnswersData };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
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
            var initialSelectPromptData = new AddRegistrationPromptSelectPromptData(1, true, "prompt");

            var initialAnswersData = new RegistrationPromptAnswersTempData(optionsString, newAnswerInput);

            var initialTempData = new AddRegistrationPromptTempData
            { SelectPromptData = initialSelectPromptData, ConfigureAnswersTempData = initialAnswersData };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
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
            var initialPromptData = new AddRegistrationPromptSelectPromptData(1, true, "prompt");

            var initialAnswersData = new RegistrationPromptAnswersTempData("Test\r\nAnswer");
            var expectedAnswersData = new RegistrationPromptAnswersTempData("Answer");

            const string action = "delete0";

            var initialTempData = new AddRegistrationPromptTempData
            { SelectPromptData = initialPromptData, ConfigureAnswersTempData = initialAnswersData };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
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
        public void AddRegistrationPromptSummaryPost_calls_registration_prompt_service_and_redirects_to_index()
        {
            // Given
            var initialPromptData = new AddRegistrationPromptSelectPromptData(1, true, "prompt");
            var initialAnswersData = new RegistrationPromptAnswersTempData("Test\r\nAnswer");
            var initialTempData = new AddRegistrationPromptTempData
            { SelectPromptData = initialPromptData, ConfigureAnswersTempData = initialAnswersData };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
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
                    () => multiPageFormService.ClearMultiPageFormData(
                        MultiPageFormDataFeature.AddRegistrationPrompt,
                        registrationPromptsController.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
        }

        [Test]
        public void
            AddRegistrationPromptSummaryPost_calls_registration_prompt_service_and_redirects_to_error_on_failure()
        {
            // Given
            var initialPromptData = new AddRegistrationPromptSelectPromptData(1, true, "prompt");
            var initialAnswersData = new RegistrationPromptAnswersTempData("Test\r\nAnswer");
            var initialTempData = new AddRegistrationPromptTempData
            { SelectPromptData = initialPromptData, ConfigureAnswersTempData = initialAnswersData };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
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
            var initialTempData = new EditRegistrationPromptTempData
            {
                PromptNumber = initialEditViewModel.PromptNumber,
                Prompt = initialEditViewModel.Prompt,
                Mandatory = initialEditViewModel.Mandatory,
                OptionsString = initialEditViewModel.OptionsString,
                Answer = initialEditViewModel.Answer,
                IncludeAnswersTableCaption = initialEditViewModel.IncludeAnswersTableCaption,
            };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<EditRegistrationPromptTempData>(
                    MultiPageFormDataFeature.EditRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).Returns(initialTempData);

            // When
            var result = registrationPromptsController.EditRegistrationPromptBulkPost(inputViewModel);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormService.SetMultiPageFormData(
                        A<EditRegistrationPromptTempData>.That.Matches(
                            d => d.PromptNumber == initialTempData.PromptNumber &&
                                 d.Prompt == initialTempData.Prompt &&
                                 d.Mandatory == initialTempData.Mandatory &&
                                 d.Answer == initialTempData.Answer &&
                                 d.IncludeAnswersTableCaption == initialTempData.IncludeAnswersTableCaption &&
                                 d.OptionsString == inputViewModel.OptionsString
                        ),
                        MultiPageFormDataFeature.EditRegistrationPrompt,
                        registrationPromptsController.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("EditRegistrationPrompt");
            }
        }

        [Test]
        public void RegistrationPromptBulkPost_updates_temp_data_and_redirects_to_configure_answers()
        {
            // Given
            var inputViewModel = new BulkRegistrationPromptAnswersViewModel("Test\r\nAnswer", true, null);
            var initialPromptData = new AddRegistrationPromptSelectPromptData(1, true, "prompt");
            var initialAnswersData = new RegistrationPromptAnswersTempData("Test");
            var expectedAnswersData = new RegistrationPromptAnswersTempData("Test\r\nAnswer");

            var initialTempData = new AddRegistrationPromptTempData
            {
                SelectPromptData = initialPromptData,
                ConfigureAnswersTempData = initialAnswersData,
            };
            A.CallTo(
                () => multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
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

        private static void AssertModelStateErrorIsExpected(IActionResult result, string expectedErrorMessage)
        {
            var errorMessage = result.As<ViewResult>().ViewData.ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0).ToList().First().First().ErrorMessage;
            errorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }

        private void AssertMultiPageFormDataIsUpdatedCorrectly(
            AddRegistrationPromptSelectPromptData expectedPromptData,
            RegistrationPromptAnswersTempData expectedAnswersTempData
        )
        {
            A.CallTo(
                () => multiPageFormService.SetMultiPageFormData(
                    A<AddRegistrationPromptTempData>.That.Matches(
                        d => d.SelectPromptData.Mandatory == expectedPromptData.Mandatory &&
                             d.SelectPromptData.CustomPromptId == expectedPromptData.CustomPromptId &&
                             d.ConfigureAnswersTempData.OptionsString == expectedAnswersTempData.OptionsString &&
                             d.ConfigureAnswersTempData.Answer == expectedAnswersTempData.Answer &&
                             d.ConfigureAnswersTempData.IncludeAnswersTableCaption ==
                             expectedAnswersTempData.IncludeAnswersTableCaption
                    ),
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    registrationPromptsController.TempData
                )
            ).MustHaveHappenedOnceExactly();
        }
    }
}
