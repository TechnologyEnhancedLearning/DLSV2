namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CentreConfiguration
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class RegistrationPromptsControllerTests
    {
        private IRequestCookieCollection cookieCollection = null!;
        private ICustomPromptsService customPromptsService = null!;
        private HttpContext httpContext = null!;
        private HttpRequest httpRequest = null!;
        private RegistrationPromptsController registrationPromptsController = null!;
        private RegistrationPromptsController registrationPromptsControllerWithMockHttpContext = null!;

        [SetUp]
        public void Setup()
        {
            customPromptsService = A.Fake<ICustomPromptsService>();

            registrationPromptsController = new RegistrationPromptsController(customPromptsService)
                .WithDefaultContext()
                .WithMockUser(true)
                .WithMockTempData();

            httpContext = A.Fake<HttpContext>();
            httpRequest = A.Fake<HttpRequest>();
            cookieCollection = A.Fake<IRequestCookieCollection>();

            var cookieList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("AddRegistrationPromptData", "AddRegistrationPromptData")
            };
            A.CallTo(() => cookieCollection.GetEnumerator()).Returns(cookieList.GetEnumerator());
            A.CallTo(() => cookieCollection.ContainsKey("AddRegistrationPromptData")).Returns(true);
            A.CallTo(() => httpRequest.Cookies).Returns(cookieCollection);
            A.CallTo(() => httpContext.Request).Returns(httpRequest);

            registrationPromptsControllerWithMockHttpContext = new RegistrationPromptsController(customPromptsService)
                .WithMockHttpContext(httpContext)
                .WithMockUser(true)
                .WithMockTempData();
        }

        [Test]
        public void PostEditRegistrationPrompt_save_calls_correct_methods()
        {
            // Given
            var model = new EditRegistrationPromptViewModel(1, "Test", false, "Test");
            const string action = "save";

            A.CallTo(
                () => customPromptsService.UpdateCustomPromptForCentre(
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
                () => customPromptsService.UpdateCustomPromptForCentre(
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
                () => customPromptsService.UpdateCustomPromptForCentre(
                    ControllerContextHelper.CentreId,
                    1,
                    false,
                    "Test"
                )
            ).DoesNothing();

            // When
            var result = registrationPromptsController.EditRegistrationPrompt(model, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditRegistrationPromptViewModel>();
                AssertNumberOfConfiguredAnswersOnView(result, 2);
            }
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
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditRegistrationPromptViewModel>();
                AssertNumberOfConfiguredAnswersOnView(result, 1);
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
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions").WithActionName("Error");
        }

        [Test]
        public void AddRegistrationPromptSelectPrompt_sets_new_temp_data()
        {
            // When
            var result = registrationPromptsControllerWithMockHttpContext.AddRegistrationPromptSelectPrompt();

            // Then
            registrationPromptsControllerWithMockHttpContext.TempData.Peek<AddRegistrationPromptData>().Should()
                .NotBeNull();
            result.Should().BeViewResult().WithDefaultViewName();
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
                AssertNumberOfConfiguredAnswersOnView(result, 2);
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
                AssertNumberOfConfiguredAnswersOnView(result, 1);
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
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions").WithActionName("Error");
        }

        [Test]
        public void AddRegistrationPromptSummary_calls_custom_prompt_service_and_redirects_to_index()
        {
            // Given
            var initialPromptModel = new AddRegistrationPromptSelectPromptViewModel(1, true);
            var initialViewModel = new RegistrationPromptAnswersViewModel("Test\r\nAnswer");
            var initialTempData = new AddRegistrationPromptData
                { SelectPromptViewModel = initialPromptModel, ConfigureAnswersViewModel = initialViewModel };
            registrationPromptsController.TempData.Set(initialTempData);
            A.CallTo(
                () => customPromptsService.AddCustomPromptToCentre(
                    ControllerContextHelper.CentreId,
                    1,
                    true,
                    "Test\r\nAnswer"
                )
            ).DoesNothing();

            // When
            var result = registrationPromptsController.AddRegistrationPromptSummaryPost();

            // Then
            A.CallTo(
                () => customPromptsService.AddCustomPromptToCentre(
                    ControllerContextHelper.CentreId,
                    1,
                    true,
                    "Test\r\nAnswer"
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        private static void AssertNumberOfConfiguredAnswersOnView(IActionResult result, int expectedCount)
        {
            result.Should().BeViewResult();
            result.As<ViewResult>().Model.As<RegistrationPromptAnswersViewModel>().Options.Count.Should()
                .Be(expectedCount);
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
    }
}
