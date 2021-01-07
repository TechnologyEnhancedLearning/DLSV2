namespace DigitalLearningSolutions.Data.Tests.Models
{
    using DigitalLearningSolutions.Data.Models;
    using FluentAssertions;
    using NUnit.Framework;

    internal class CourseSettingsTests
    {
        [Test]
        public void CourseSettings_should_have_default_settings_when_json_is_malformed()
        {
            // Given
            var defaultSettings = new CourseSettings(null);

            // When
            var courseSettings = new CourseSettings("{\"lm.su\":\"Line Manager");

            // Then
            courseSettings.Should().BeEquivalentTo(defaultSettings);
        }

        [Test]
        public void CourseSettings_should_have_default_settings_when_json_is_empty_string()
        {
            // Given
            var defaultSettings = new CourseSettings(null);

            // When
            var courseSettings = new CourseSettings("");

            // Then
            courseSettings.Should().BeEquivalentTo(defaultSettings);
        }

        [Test]
        public void CourseSettings_should_have_default_settings_when_json_is_empty_whitespace()
        {
            // Given
            var defaultSettings = new CourseSettings(null);

            // When
            var courseSettings = new CourseSettings("   ");

            // Then
            courseSettings.Should().BeEquivalentTo(defaultSettings);
        }

        [Test]
        public void CourseSettings_should_have_default_settings_when_json_is_empty_object()
        {
            // Given
            var defaultSettings = new CourseSettings(null);

            // When
            var courseSettings = new CourseSettings("{}");

            // Then
            courseSettings.Should().BeEquivalentTo(defaultSettings);
        }

        [Test]
        public void CourseSettings_should_have_default_settings_when_json_is_empty_list()
        {
            // Given
            var defaultSettings = new CourseSettings(null);

            // When
            var courseSettings = new CourseSettings("[]");

            // Then
            courseSettings.Should().BeEquivalentTo(defaultSettings);
        }

        [Test]
        public void CourseSettings_should_take_ShowPercentage_from_json()
        {
            // When
            var courseSettings = new CourseSettings("{\"lm.sp\":false}");

            // Then
            courseSettings.ShowPercentage.Should().BeFalse();
        }

        [Test]
        public void CourseSettings_should_have_default_ShowPercentage_when_not_in_json()
        {
            // When
            var courseSettings = new CourseSettings(null);

            // Then
            courseSettings.ShowPercentage.Should().BeTrue();
        }

        [Test]
        public void CourseSettings_should_have_default_ShowPercentage_when_wrong_type()
        {
            // When
            var courseSettings = new CourseSettings("{\"lm.sp\":\"Line Manager\"}");

            // Then
            courseSettings.ShowPercentage.Should().BeTrue();
        }

        [Test]
        public void CourseSettings_should_take_ShowTime_from_json()
        {
            // When
            var courseSettings = new CourseSettings("{\"lm.st\":false}");

            // Then
            courseSettings.ShowTime.Should().BeFalse();
        }

        [Test]
        public void CourseSettings_should_have_default_ShowTime_when_not_in_json()
        {
            // When
            var courseSettings = new CourseSettings(null);

            // Then
            courseSettings.ShowTime.Should().BeTrue();
        }

        [Test]
        public void CourseSettings_should_have_default_ShowTime_when_wrong_type()
        {
            // When
            var courseSettings = new CourseSettings("{\"lm.st\":3}");

            // Then
            courseSettings.ShowTime.Should().BeTrue();
        }

        [Test]
        public void CourseSettings_should_take_ShowLearnStatus_from_json()
        {
            // When
            var courseSettings = new CourseSettings("{\"lm.sl\":false}");

            // Then
            courseSettings.ShowLearnStatus.Should().BeFalse();
        }

        [Test]
        public void CourseSettings_should_have_default_ShowLearnStatus_when_not_in_json()
        {
            // When
            var courseSettings = new CourseSettings(null);

            // Then
            courseSettings.ShowLearnStatus.Should().BeTrue();
        }

        [Test]
        public void CourseSettings_should_have_default_ShowLearnStatus_when_wrong_type()
        {
            // When
            var courseSettings = new CourseSettings("{\"lm.sl\":\"Line Manager\"}");

            // Then
            courseSettings.ShowLearnStatus.Should().BeTrue();
        }

        [Test]
        public void CourseSettings_should_take_ConsolidationExercise_from_json()
        {
            // When
            var courseSettings = new CourseSettings("{\"lm:ce\":\"Consolidation exercise\"}");

            // Then
            courseSettings.ConsolidationExercise.Should().Be("Consolidation exercise");
        }

        [Test]
        public void CourseSettings_should_have_default_ConsolidationExercise_when_not_in_json()
        {
            // When
            var courseSettings = new CourseSettings(null);

            // Then
            courseSettings.ConsolidationExercise.Should().BeNull();
        }

        [Test]
        public void CourseSettings_should_have_default_ConsolidationExercise_when_wrong_type()
        {
            // When
            var courseSettings = new CourseSettings("{\"lm:ce\":false}");

            // Then
            courseSettings.ConsolidationExercise.Should().BeNull();
        }
    }
}
