namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CentreConfiguration
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class RegistrationPromptsControllerTests
    {
        private ICentresDataService centresDataService = null!;
        private ICustomPromptsService customPromptsService = null!;
        private CentreConfigurationController centreConfigurationController = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            customPromptsService = A.Fake<ICustomPromptsService>();

            centreConfigurationController = new CentreConfigurationController(centresDataService, customPromptsService)
                .WithDefaultContext().WithMockUser(true);
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

            A.CallTo(() => customPromptsService.UpdateCustomPromptForCentre(ControllerContextHelper.CentreId, 1, false, "Test")).DoesNothing();

            // When
            var result = centreConfigurationController.EditRegistrationPrompt(model, action);

            // Then
            A.CallTo(() => customPromptsService.UpdateCustomPromptForCentre(ControllerContextHelper.CentreId, 1, false, "Test")).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("RegistrationPrompts");
        }

        [Test]
        public void PostEditRegistrationPrompt_add_configures_new_answer()
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

            A.CallTo(() => customPromptsService.UpdateCustomPromptForCentre(ControllerContextHelper.CentreId, 1, false, "Test")).DoesNothing();

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
        public void PostEditRegistrationPrompt_delete_removes_configured_answer()
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
