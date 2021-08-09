namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using FluentAssertions;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        [Test]
        public void GetAdminCountWithAnswerForPrompt_returns_expected_count()
        {
            // When
            var count = userDataService.GetAdminCountWithAnswerForPrompt(357, 1);

            // Then
            count.Should().Be(1);
        }
    }
}
