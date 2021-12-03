namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class StringHelperTests
    {
        [Test]
        public void ReplaceNoneAlphaNumericSpaceChars_null_input_returns_null()
        {
            // When
            var result = StringHelper.ReplaceNoneAlphaNumericSpaceChars(null, "a");

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void ReplaceNoneAlphaNumericSpaceChars_returns_cleaned_string_with_replacement()
        {
            // Given
            var input = "abcdefghijklmnopqrstuvwxyz" +
                        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                        "1234567890" +
                        "`¬¦!\"£$%^&*)(_+-=[]{};'#:@~\\|,./<>? ";

            var expectedOutput = "abcdefghijklmnopqrstuvwxyz" +
                                 "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                 "1234567890" +
                                 "rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr ";

            // When
            var result = StringHelper.ReplaceNoneAlphaNumericSpaceChars(input, "r");

            // Then
            result.Should().Be(expectedOutput);
        }
    }
}
