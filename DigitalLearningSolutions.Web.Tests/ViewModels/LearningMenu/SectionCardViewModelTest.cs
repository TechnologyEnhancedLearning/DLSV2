namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    class SectionCardViewModelTest
    {
        [Test]
        public void Section_should_return_empty_string_if_has_learning_is_false()
        {
            // Given
            const string sectionName = "TestName";
            const bool hasLearning = false;
            const double percentComplete = 12.00;
            var section = new CourseSection(sectionName, hasLearning, percentComplete);

            // When
            var sectionCardViewModel = new SectionCardViewModel(section);

            // Then
            sectionCardViewModel.HasLearning.Should().BeFalse();
            sectionCardViewModel.GetPercentComplete().Should().Be("");
        }

        [Test]
        public void Section_should_return_correct_string_if_has_learning_is_true()
        {
            // Given
            const string sectionName = "TestName";
            const bool hasLearning = true;
            const double percentComplete = 12.00;
            var section = new CourseSection(sectionName, hasLearning, percentComplete);

            // When
            var sectionCardViewModel = new SectionCardViewModel(section);

            // Then
            sectionCardViewModel.HasLearning.Should().BeTrue();
            sectionCardViewModel.GetPercentComplete().Should().Be($"{percentComplete}% Complete");
        }
    }
}
