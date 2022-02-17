namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using DigitalLearningSolutions.Data.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class NameQueryHelperTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetSortableFullName_returns_last_name_only_when_first_name_is_null_or_whitespace(string? firstName)
        {
            // Given
            const string lastName = "Dickinson";

            // When
            var result = NameQueryHelper.GetSortableFullName(firstName, lastName);

            // Then
            result.Should().Be(lastName);
        }

        [Test]
        public void GetSortableFullName_returns_correctly_formatted_name_when_both_names_provided()
        {
            // Given
            const string lastName = "Dickinson";
            const string firstName = "Bruce";

            // When
            var result = NameQueryHelper.GetSortableFullName(firstName, lastName);

            // Then
            result.Should().Be("Dickinson, Bruce");
        }
    }
}
