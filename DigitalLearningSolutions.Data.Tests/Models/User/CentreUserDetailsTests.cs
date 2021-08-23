namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using DigitalLearningSolutions.Data.Models.User;
    using FluentAssertions;
    using NUnit.Framework;

    public class CentreUserDetailsTests
    {
        [TestCase(true, false, "Admin")]
        [TestCase(false, true, "Delegate")]
        [TestCase(true, true, "Admin, Delegate")]
        public void CentreUserDetails_sets_tags_string_correctly(
            bool isAdmin,
            bool isDelegate,
            string tagString
        )
        {
            // When
            var result = new CentreUserDetails(1, "centre", isAdmin, isDelegate);

            // Then
            result.TagString.Should().Be(tagString);
        }
    }
}
