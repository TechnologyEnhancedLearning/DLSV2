﻿namespace DigitalLearningSolutions.Data.Tests.Models
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CustomPromptTests
    {
        [Test]
        public void CustomPrompt_constructor_populates_options_with_null()
        {
            // When
            var result = new CustomPrompt("Test", null, false);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(0);
            }
        }

        [Test]
        public void CustomPrompt_constructor_populates_options_with_single_entry()
        {
            // When
            var result = new CustomPrompt("Test", "Option", false);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(1);
            }
        }

        [Test]
        public void CustomPrompt_constructor_populates_options_with_several_newline_separated_entries()
        {
            // Given
            var options = "Option1\r\nOption2\r\nOption3\r\nOption4";

            // When
            var result = new CustomPrompt("Test", options, false);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(4);
            }
        }

        [Test]
        public void CustomPrompt_constructor_populates_options_with_extra_newline_separated_entries()
        {
            // Given
            var options = "Option1\r\nOption2\r\n\r\nOption3\r\n\r\n\r\n\r\nOption4";

            // When
            var result = new CustomPrompt("Test", options, false);

            // Then
            using (new AssertionScope())
            {
                result.Options.Should().NotBeNull();
                result.Options.Count.Should().Be(4);
            }
        }
    }
}
