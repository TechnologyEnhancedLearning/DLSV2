namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;
    using ConfigurationExtensions = Data.Extensions.ConfigurationExtensions;


    public class StringHelperTests
    {
        private readonly IConfiguration config = A.Fake<IConfiguration>();

        [TestCase("https://hee-dls-test.softwire.com", "%2fMyAccount")]
        [TestCase("https://hee-dls-test.softwire.com/uar-test", "%2fuar-test%2fMyAccount")]
        public void GetLocalRedirectUrl_returns_correctly_formatted_url(string appRootPath, string expectedReturnValue)
        {
            // Given
            A.CallTo(() => config[ConfigurationExtensions.AppRootPathName]).Returns(appRootPath);
            const string endpointToRedirectTo = "/MyAccount";

            // When
            var result = StringHelper.GetLocalRedirectUrl(config, endpointToRedirectTo);

            // Then
            result.Should().Be(expectedReturnValue);
        }
    }
}
