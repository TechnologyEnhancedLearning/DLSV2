namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class RegistrationMappingHelperTests
    {
        [Test]
        public void MapToDelegateRegistrationModel_returns_correct_DelegateRegistrationModel()
        {
            // Given
            var data = RegistrationDataHelper.SampleDelegateRegistrationData();

            // When
            var result = RegistrationMappingHelper.MapSelfRegistrationToDelegateRegistrationModel(data);

            // Then
            result.FirstName.Should().Be(data.FirstName);
            result.LastName.Should().Be(data.LastName);
            result.PrimaryEmail.Should().Be(data.PrimaryEmail);
            result.Centre.Should().Be(data.Centre);
            result.JobGroup.Should().Be(data.JobGroup);
            result.PasswordHash.Should().Be(data.PasswordHash);
            result.Answer1.Should().Be(data.Answer1);
            result.Answer2.Should().Be(data.Answer2);
            result.Answer3.Should().Be(data.Answer3);
        }
    }
}
