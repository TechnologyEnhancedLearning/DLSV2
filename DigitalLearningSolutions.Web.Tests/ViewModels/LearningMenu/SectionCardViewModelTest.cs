namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    class SectionCardViewModelTest
    {
        const int CustomisationId = 1;

        [Test]
        public void Section_should_return_empty_string_if_has_learning_is_false()
        {
            // Given
            const bool hasLearning = false;
            const bool showPercentageCourseSetting = true;

            var section = CourseSectionHelper.CreateDefaultCourseSection(hasLearning: hasLearning);

            // When
            var sectionCardViewModel = new SectionCardViewModel(section, CustomisationId, showPercentageCourseSetting);

            // Then
            sectionCardViewModel.PercentComplete.Should().Be("");
        }

        [Test]
        public void Section_should_return_correct_string_if_has_learning_is_true()
        {
            // Given
            const bool hasLearning = true;
            const double percentComplete = 12.00;
            const bool showPercentageCourseSetting = true;

            var section = CourseSectionHelper.CreateDefaultCourseSection(
                hasLearning: hasLearning,
                percentComplete: percentComplete
            );

            // When
            var sectionCardViewModel = new SectionCardViewModel(section, CustomisationId, showPercentageCourseSetting);

            // Then
            sectionCardViewModel.PercentComplete.Should().Be($"{percentComplete}% learning complete");
        }

        [Test]
        public void Section_should_return_empty_string_if_showPercentageCourseSetting_is_false()
        {
            // Given
            const bool hasLearning = true;
            const double percentComplete = 12.00;
            const bool showPercentageCourseSetting = false;

            var section = CourseSectionHelper.CreateDefaultCourseSection(
                hasLearning: hasLearning,
                percentComplete: percentComplete
            );

            // When
            var sectionCardViewModel = new SectionCardViewModel(section, CustomisationId, showPercentageCourseSetting);

            // Then
            sectionCardViewModel.PercentComplete.Should().Be("");
        }

        [Test]
        public void Section_should_return_customisation_id()
        {
            // Given
            const int customisationId = 10;
            const bool showPercentageCourseSetting = true;
            var section = CourseSectionHelper.CreateDefaultCourseSection();

            // When
            var sectionCardViewModel = new SectionCardViewModel(section, customisationId, showPercentageCourseSetting);

            // Then
            sectionCardViewModel.CustomisationId.Should().Be(customisationId);
        }

        [Test]
        public void Section_should_floor_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            const double percentComplete = 16.6666666667;
            const double percentCompleteRounded = 16;
            const bool showPercentageCourseSetting = true;
            var section = CourseSectionHelper.CreateDefaultCourseSection(
                hasLearning: hasLearning,
                percentComplete: percentComplete
            );

            // When
            var sectionCardViewModel = new SectionCardViewModel(section, CustomisationId, showPercentageCourseSetting);

            // Then
            sectionCardViewModel.PercentComplete.Should().Be($"{percentCompleteRounded}% learning complete");
        }
    }
}
