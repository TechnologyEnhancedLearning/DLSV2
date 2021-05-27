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
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
                .WithMockUser(true);
            registrationPromptsController.TempData = new TempDataDictionary
                (registrationPromptsController.ControllerContext.HttpContext, A.Fake<ITempDataProvider>());

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
                .WithMockUser(true);
            registrationPromptsControllerWithMockHttpContext.TempData = new TempDataDictionary
                (httpContext, A.Fake<ITempDataProvider>());
        }

        [Test]
        public void PostEditRegistrationPrompt_save_calls_correct_methods()
        {
            // Given
            var model = new EditRegistrationPromptViewModel
            {
                Answer = null,
                Mandatory = false,
                OptionsString = "Test",
                Prompt = "Test",
                PromptNumber = 1
            };
            const string action = "save";

            A.CallTo
            (
                () => customPromptsService.UpdateCustomPromptForCentre
                    (ControllerContextHelper.CentreId, 1, false, "Test")
            ).DoesNothing();

            // When
            var result = registrationPromptsController.EditRegistrationPrompt(model, action);

            // Then
            A.CallTo
            (
                () => customPromptsService.UpdateCustomPromptForCentre
                    (ControllerContextHelper.CentreId, 1, false, "Test")
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void PostEditRegistrationPrompt_add_configures_new_answer()
        {
            // Given
            var model = new EditRegistrationPromptViewModel
            {
                Answer = "Answer",
                Mandatory = false,
                OptionsString = "Test",
                Prompt = "Test",
                PromptNumber = 1
            };
            const string action = "addPrompt";

            A.CallTo
            (
                () => customPromptsService.UpdateCustomPromptForCentre
                    (ControllerContextHelper.CentreId, 1, false, "Test")
            ).DoesNothing();

            // When
            var result = registrationPromptsController.EditRegistrationPrompt(model, action);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult();
                result.As<ViewResult>().Model.Should().BeOfType<EditRegistrationPromptViewModel>();
                result.As<ViewResult>().Model.As<EditRegistrationPromptViewModel>().Options.Count.Should().Be(2);
            }
        }

        [Test]
        public void PostEditRegistrationPrompt_delete_removes_configured_answer()
        {
            // Given
            var model = new EditRegistrationPromptViewModel
            {
                Answer = "Answer",
                Mandatory = false,
                OptionsString = "Test\r\nAnswer",
                Prompt = "Test",
                PromptNumber = 1
            };
            const string action = "delete0";

            // When
            var result = registrationPromptsController.EditRegistrationPrompt(model, action);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult();
                result.As<ViewResult>().Model.Should().BeOfType<EditRegistrationPromptViewModel>();
                result.As<ViewResult>().Model.As<EditRegistrationPromptViewModel>().Options.Count.Should().Be(1);
            }
        }

        [Test]
        public void PostEditRegistrationPrompt_returns_error_with_unexpected_action()
        {
            // Given
            var model = new EditRegistrationPromptViewModel
            {
                Answer = "Answer",
                Mandatory = false,
                OptionsString = "Test\r\nAnswer",
                Prompt = "Test",
                PromptNumber = 1
            };
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
            var expectedPromptModel = new AddRegistrationPromptSelectPromptViewModel
            {
                CustomPromptId = 1,
                Mandatory = true
            };
            var initialTempData = new AddRegistrationPromptData();
            registrationPromptsController.TempData.Set(initialTempData);

            // When
            var result = registrationPromptsController.AddRegistrationPromptSelectPrompt(expectedPromptModel);

            // Then
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>().Should().NotBeNull();
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.SelectPromptViewModel.Should()
                .BeEquivalentTo(expectedPromptModel);
            result.Should().BeRedirectToActionResult().WithActionName("AddRegistrationPromptConfigureAnswers");
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_next_updates_temp_data()
        {
            // Given
            var expectedPromptModel = new AddRegistrationPromptSelectPromptViewModel
            {
                CustomPromptId = 1,
                Mandatory = true
            };
            var initialTempData = new AddRegistrationPromptData { SelectPromptViewModel = expectedPromptModel };
            registrationPromptsController.TempData.Set(initialTempData);
            var model = new RegistrationPromptAnswersViewModel
            {
                Answer = null,
                OptionsString = "Test"
            };
            const string action = "next";

            // When
            var result = registrationPromptsController.AddRegistrationPromptConfigureAnswers(model, action);

            // Then
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>().Should().NotBeNull();
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.SelectPromptViewModel.Should()
                .BeEquivalentTo(expectedPromptModel);
            registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.ConfigureAnswersViewModel.Should()
                .BeEquivalentTo(model);
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_add_configures_new_answer_and_updates_temp_data()
        {
            // Given
            var model = new RegistrationPromptAnswersViewModel
            {
                Answer = "Answer",
                OptionsString = "Test"
            };
            var expectedPromptModel = new AddRegistrationPromptSelectPromptViewModel
            {
                CustomPromptId = 1,
                Mandatory = true
            };
            var initialTempData = new AddRegistrationPromptData
                { SelectPromptViewModel = expectedPromptModel, ConfigureAnswersViewModel = model };
            registrationPromptsController.TempData.Set(initialTempData);
            var expectedViewModel = new RegistrationPromptAnswersViewModel
            {
                OptionsString = "Test\r\nAnswer"
            };
            const string action = "addPrompt";

            // When
            var result = registrationPromptsController.AddRegistrationPromptConfigureAnswers(model, action);

            // Then
            using (new AssertionScope())
            {
                registrationPromptsController.TempData.Peek<AddRegistrationPromptData>().Should().NotBeNull();
                registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.SelectPromptViewModel.Should()
                    .BeEquivalentTo(expectedPromptModel);
                registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.ConfigureAnswersViewModel
                    .Should().BeEquivalentTo(expectedViewModel);
                result.Should().BeViewResult();
                result.As<ViewResult>().Model.Should().BeOfType<RegistrationPromptAnswersViewModel>();
                result.As<ViewResult>().Model.As<RegistrationPromptAnswersViewModel>().Options.Count.Should().Be(2);
            }
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_delete_removes_configured_answer()
        {
            // Given
            var model = new RegistrationPromptAnswersViewModel
            {
                OptionsString = "Test\r\nAnswer"
            };
            const string action = "delete0";
            var expectedPromptModel = new AddRegistrationPromptSelectPromptViewModel
            {
                CustomPromptId = 1,
                Mandatory = true
            };
            var initialTempData = new AddRegistrationPromptData
                { SelectPromptViewModel = expectedPromptModel, ConfigureAnswersViewModel = model };
            registrationPromptsController.TempData.Set(initialTempData);
            var expectedViewModel = new RegistrationPromptAnswersViewModel
            {
                OptionsString = "Answer"
            };

            // When
            var result = registrationPromptsController.AddRegistrationPromptConfigureAnswers(model, action);

            // Then
            using (new AssertionScope())
            {
                registrationPromptsController.TempData.Peek<AddRegistrationPromptData>().Should().NotBeNull();
                registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.SelectPromptViewModel.Should()
                    .BeEquivalentTo(expectedPromptModel);
                registrationPromptsController.TempData.Peek<AddRegistrationPromptData>()!.ConfigureAnswersViewModel
                    .Should().BeEquivalentTo(expectedViewModel);
                result.Should().BeViewResult();
                result.As<ViewResult>().Model.Should().BeOfType<RegistrationPromptAnswersViewModel>();
                result.As<ViewResult>().Model.As<RegistrationPromptAnswersViewModel>().Options.Count.Should().Be(1);
            }
        }

        [Test]
        public void AddRegistrationPromptConfigureAnswers_returns_error_with_unexpected_action()
        {
            // Given
            var model = new RegistrationPromptAnswersViewModel
            {
                Answer = "Answer",
                OptionsString = "Test\r\nAnswer"
            };
            const string action = "deletetest";

            // When
            var result = registrationPromptsController.AddRegistrationPromptConfigureAnswers(model, action);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions").WithActionName("Error");
        }
    }
}
