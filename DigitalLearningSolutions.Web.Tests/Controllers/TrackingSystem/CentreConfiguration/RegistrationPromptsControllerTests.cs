namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CentreConfiguration
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class RegistrationPromptsControllerTests
    {
        private ICentresDataService centresDataService;
        private ICustomPromptsService customPromptsService;
        private CentreConfigurationController centreConfigurationController;
        private ISession httpContextSession;
        private const int CentreId = 2;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            customPromptsService = A.Fake<ICustomPromptsService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("UserCentreID", CentreId.ToString())
            }, "mock"));
            httpContextSession = new MockHttpContextSession();

            centreConfigurationController = new CentreConfigurationController(centresDataService, customPromptsService)
                .WithDefaultContext().WithMockUser(true, CentreId);
        }

        [Test]
        public void PostEditRegistrationPrompt_save_calls_correct_methods()
        {
            // Given
            var model = new EditRegistrationPromptViewModel
            {
                Answer = null,
                Mandatory = false,
                Options = null,
                OptionsString = "Test",
                Prompt = "Test",
                PromptNumber = 1
            };
            const string action = "save";

            A.CallTo(() => customPromptsService.UpdateCustomPromptForCentre(CentreId, 1, false, "Test")).DoesNothing();

            // When
            var result = centreConfigurationController.EditRegistrationPrompt(model, action);

            // Then
            A.CallTo(() => customPromptsService.UpdateCustomPromptForCentre(CentreId, 1, false, "Test")).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("RegistrationPrompts");
        }

        [Test]
        public void PostEditRegistrationPrompt_add_produces_expected_result()
        {
            // Given
            var model = new EditRegistrationPromptViewModel
            {
                Answer = "Answer",
                Mandatory = false,
                Options = null,
                OptionsString = "Test",
                Prompt = "Test",
                PromptNumber = 1
            };
            const string action = "addPrompt";

            A.CallTo(() => customPromptsService.UpdateCustomPromptForCentre(CentreId, 1, false, "Test")).DoesNothing();

            // When
            var result = centreConfigurationController.EditRegistrationPrompt(model, action);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult();
                result.As<ViewResult>().Model.Should().BeOfType<EditRegistrationPromptViewModel>();
                result.As<ViewResult>().Model.As<EditRegistrationPromptViewModel>().Options?.Count.Should().Be(2);
            }
        }

        [Test]
        public void PostEditRegistrationPrompt_delete_produces_expected_result()
        {
            // Given
            var model = new EditRegistrationPromptViewModel
            {
                Answer = "Answer",
                Mandatory = false,
                Options = null,
                OptionsString = "Test\r\nAnswer",
                Prompt = "Test",
                PromptNumber = 1
            };
            const string action = "delete0";

            // When
            var result = centreConfigurationController.EditRegistrationPrompt(model, action);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult();
                result.As<ViewResult>().Model.Should().BeOfType<EditRegistrationPromptViewModel>();
                result.As<ViewResult>().Model.As<EditRegistrationPromptViewModel>().Options?.Count.Should().Be(1);
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
                Options = null,
                OptionsString = "Test\r\nAnswer",
                Prompt = "Test",
                PromptNumber = 1
            };
            const string action = "deletetest";

            // When
            var result = centreConfigurationController.EditRegistrationPrompt(model, action);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions").WithActionName("Error");
        }
    }
}
