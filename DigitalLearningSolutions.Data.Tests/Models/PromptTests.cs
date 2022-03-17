namespace DigitalLearningSolutions.Data.Tests.Models
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class PromptTests
    {
        [Test]
        public void CentreRegistrationPrompt_constructor_populates_options_with_null()
        {
            // When
            var result = new CentreRegistrationPrompt(1, 1,"Test", null, false);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(0);
            }
        }

        [Test]
        public void CentreRegistrationPrompt_constructor_populates_options_with_single_entry()
        {
            // When
            var result = new CentreRegistrationPrompt(1, 1,"Test", "Option", false);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(1);
            }
        }

        [Test]
        public void CentreRegistrationPrompt_constructor_populates_options_with_several_newline_separated_entries()
        {
            // Given
            var options = "Option1\r\nOption2\r\nOption3\r\nOption4";

            // When
            var result = new CentreRegistrationPrompt(1, 1,"Test", options, false);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(4);
            }
        }

        [Test]
        public void CentreRegistrationPrompt_constructor_populates_options_with_extra_newline_separated_entries()
        {
            // Given
            var options = "Option1\r\nOption2\r\n\r\nOption3\r\n\r\n\r\n\r\nOption4";

            // When
            var result = new CentreRegistrationPrompt(1, 1,"Test", options, false);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(4);
            }
        }

        [Test]
        public void CourseAdminField_constructor_populates_options_with_null()
        {
            // When
            var result = new CourseAdminField(1,"Test", null);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(0);
            }
        }

        [Test]
        public void CourseAdminField_constructor_populates_options_with_single_entry()
        {
            // When
            var result = new CourseAdminField(1,"Test", "Option");

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(1);
            }
        }

        [Test]
        public void CourseAdminField_constructor_populates_options_with_several_newline_separated_entries()
        {
            // Given
            var options = "Option1\r\nOption2\r\nOption3\r\nOption4";

            // When
            var result = new CourseAdminField(1,"Test", options);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(4);
            }
        }

        [Test]
        public void CourseAdminField_constructor_populates_options_with_extra_newline_separated_entries()
        {
            // Given
            var options = "Option1\r\nOption2\r\n\r\nOption3\r\n\r\n\r\n\r\nOption4";

            // When
            var result = new CourseAdminField(1,"Test", options);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(4);
            }
        }
    }
}
