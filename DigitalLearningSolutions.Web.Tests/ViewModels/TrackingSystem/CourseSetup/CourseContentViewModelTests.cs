namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseContentViewModelTests
    {
        private static readonly Tutorial DisabledTutorial = TutorialTestHelper.GetDefaultTutorial(status: false, diagStatus: false);

        private static readonly Section DisabledSection =
            new Section(1, "disabled", new List<Tutorial> { DisabledTutorial, DisabledTutorial });

        [Test]
        public void CourseHasContent_is_true_when_at_least_one_section_has_learning_assessment_tutorial()
        {
            // Given
            var enabledTutorial = TutorialTestHelper.GetDefaultTutorial(diagStatus: false);
            var enabledSection = new Section(1, "test", new List<Tutorial> { enabledTutorial, DisabledTutorial });

            // When
            var viewModel = new CourseContentViewModel(
                1,
                "course",
                false,
                new List<Section> { enabledSection, DisabledSection }
            );

            // Then
            viewModel.CourseHasContent.Should().BeTrue();
        }

        [Test]
        public void CourseHasContent_is_true_when_at_least_one_section_has_diagnostic_assessment_tutorial()
        {
            // Given
            var enabledTutorial = TutorialTestHelper.GetDefaultTutorial(status: false);
            var enabledSection = new Section(1, "test", new List<Tutorial> { enabledTutorial, DisabledTutorial });

            // When
            var viewModel = new CourseContentViewModel(
                1,
                "course",
                false,
                new List<Section> { enabledSection, DisabledSection }
            );

            // Then
            viewModel.CourseHasContent.Should().BeTrue();
        }

        [Test]
        public void CourseHasContent_is_true_when_all_tutorials_disabled_but_has_post_learning_assessment()
        {
            // When
            var viewModel = new CourseContentViewModel(1, "course", true, new List<Section> { DisabledSection });

            // Then
            viewModel.CourseHasContent.Should().BeTrue();
        }

        [Test]
        public void CourseHasContent_is_false_when_all_tutorials_disabled_and_has_no_post_learning_assessment()
        {
            // When
            var viewModel = new CourseContentViewModel(1, "course", false, new List<Section> { DisabledSection });

            // Then
            viewModel.CourseHasContent.Should().BeFalse();
        }

        [Test]
        public void CourseHasContent_is_false_when_no_sections_are_present()
        {
            // When
            var viewModel = new CourseContentViewModel(1, "course", true, new List<Section>());

            // Then
            viewModel.CourseHasContent.Should().BeFalse();
        }
    }
}
