namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
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
        private readonly ICustomPromptsService customPromptsService = A.Fake<ICustomPromptsService>();
        private AdminFieldsController controller = null!;

        [SetUp]
        public void Setup()
        {
            controller = new AdminFieldsController(customPromptsService)
                .WithDefaultContext()
                .WithMockUser(true, 101)
                .WithMockTempData();
        }

        [Test]
        public void AdminFields_returns_NotFound_when_no_appropriate_course_found()
        {
            // Given
            A.CallTo(() => customPromptsService.GetCustomPromptsForCourse(A<int>._, A<int>._, A<int>._))
                .Returns(null);

            // When
            var result = controller.AdminFields(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void AdminFields_returns_AdminFields_page_when_appropriate_course_found()
        {
            // Given
            var samplePrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, "System Access Granted", "Yes\r\nNo");
            var customPrompts = new List<CustomPrompt> { samplePrompt1 };
            A.CallTo(() => customPromptsService.GetCustomPromptsForCourse(A<int>._, A<int>._, A<int>._))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseCustomPrompts(customPrompts));

            // When
            var result = controller.AdminFields(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<AdminFieldsViewModel>();
        }

        [Test]
        public void PostEditAdminField_save_calls_correct_methods()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, 1, "Test", "Options", false);
            const string action = "save";

            A.CallTo(
                () => customPromptsService.UpdateCustomPromptForCourse(
                    1,
                    1,
                    false,
                    "Options"
                )
            ).DoesNothing();

            // When
            var result = controller.EditAdminField(model, action);

            // Then
            A.CallTo(
                () => customPromptsService.UpdateCustomPromptForCourse(
                    1,
                    1,
                    false,
                    "Options"
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("AdminFields");
        }

        [Test]
        public void PostEditAdminField_add_configures_new_answer()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, 1, "Test", "Options", false);
            const string action = "addPrompt";

            A.CallTo(
                () => customPromptsService.UpdateCustomPromptForCourse(
                    1,
                    1,
                    false,
                    "Test"
                )
            ).DoesNothing();

            // When
            var result = controller.EditAdminField(model, action);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditAdminFieldViewModel>();
                AssertNumberOfConfiguredAnswersOnView(result, 2);
            }
        }

        [Test]
        public void PostEditRegistrationPrompt_delete_removes_configured_answer()
        {
            // Given
            var prompt = new CustomPrompt(1, "Test", "Test\r\nAnswer", false);
            var model = new EditAdminFieldViewModel(prompt, 1);
            const string action = "delete0";

            // When
            var result = controller.EditAdminField(model, action);

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
            var model = new EditAdminFieldViewModel(1, 1, "Test", "Options", false);
            const string action = "bulk";

            // When
            var result = controller.EditAdminField(model, action);

            // Then
            using (new AssertionScope())
            {
                AssertEditTempDataIsExpected(model);
                result.Should().BeRedirectToActionResult().WithActionName("EditAdminFieldBulk");
            }
        }

        [Test]
        public void PostEditAdminField_returns_error_with_unexpected_action()
        {
            // Given
            var model = new EditAdminFieldViewModel(1, 1, "Test", "Options", false);
            const string action = "deletetest";

            // When
            var result = controller.EditAdminField(model, action);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions").WithActionName("Error");
        }

        [Test]
        public void AdminFieldBulkPost_updates_temp_data_and_redirects_to_edit()
        {
            // Given
            var inputViewModel = new BulkAdminFieldAnswersViewModel("Test\r\nAnswer", false, 1, 1);
            var initialEditViewModel = new EditAdminFieldViewModel(1, 1, "Test", "Test", false);
            var expectedViewModel = new EditAdminFieldViewModel(1, 1, "Test", "Test\r\nAnswer", false);
            var initialTempData = new EditAdminFieldData(initialEditViewModel);

            controller.TempData.Set(initialTempData);

            // When
            var result = controller.EditAdminFieldBulkPost(inputViewModel);

            // Then
            using (new AssertionScope())
            {
                AssertEditTempDataIsExpected(expectedViewModel);
                result.Should().BeRedirectToActionResult().WithActionName("EditAdminField");
            }
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
    }
}
