namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using FluentAssertions;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        [Test]
        public void GetAnswerCountForCourseAdminField_returns_expected_count()
        {
            // When
            var count = userDataService.GetAnswerCountForCourseAdminField(100, 1);

            // Then
            count.Should().Be(1);
        }
    }
}
