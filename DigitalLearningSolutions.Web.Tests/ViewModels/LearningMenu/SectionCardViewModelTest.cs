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
            const bool hasLearning = false;
            var section = CourseSectionHelper.CreateDefaultCourseSection(hasLearning: hasLearning);

            // When
            var sectionCardViewModel = new SectionCardViewModel(section);

            // Then
            sectionCardViewModel.PercentComplete.Should().Be("");
        }

        [Test]
        public void Section_should_return_correct_string_if_has_learning_is_true()
        {
            // Given
            const bool hasLearning = true;
            const double percentComplete = 12.00;
            var section = CourseSectionHelper.CreateDefaultCourseSection(
                hasLearning: hasLearning,
                percentComplete: percentComplete
                );

            // When
            var sectionCardViewModel = new SectionCardViewModel(section);

            // Then
            sectionCardViewModel.PercentComplete.Should().Be($"{percentComplete}% Complete");
        }
    }
}
