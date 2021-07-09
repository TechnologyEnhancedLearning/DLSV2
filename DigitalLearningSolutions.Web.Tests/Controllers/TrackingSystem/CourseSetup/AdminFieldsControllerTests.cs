namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
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
                .WithMockUser(true, 101);
        }

        [Test]
        public void AdminFields_returns_NotFound_when_no_appropriate_course_found()
        {
            // Given
            A.CallTo(() => customPromptsService.GetCustomPromptsForCourse(A<int>._, A<int>._, A<int>._)).Returns(null);

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
    }
}
