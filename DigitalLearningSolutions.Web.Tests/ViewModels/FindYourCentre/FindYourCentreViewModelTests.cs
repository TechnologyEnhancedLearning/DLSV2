namespace DigitalLearningSolutions.Web.Tests.ViewModels.FindYourCentre
{
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class FindYourCentreViewModelTests
    {
        private IConfiguration config = null!;

        [SetUp]
        public void Setup()
        {
            config = A.Fake<IConfiguration>();
        }

        [Test]
        public void FindYourCentreViewModel_without_centreid_should_have_default_Url()
        {
            // Given
            var defaultUrl = $"{config.GetCurrentSystemBaseUrl()}/findyourcentre?nonav=true";

            // When
            var model = new FindYourCentreViewModel(config);

            // Then
            model.Url.Should().BeEquivalentTo(defaultUrl);
        }

        [Test]
        public void FindYourCentreViewModel_with_centreid_should_have_centreid_added_to_Url()
        {
            // Given
            var centreId = "5";
            var defaultUrl = $"{config.GetCurrentSystemBaseUrl()}/findyourcentre?nonav=true&centreid={centreId}";

            // When
            var model = new FindYourCentreViewModel(centreId, config);

            // Then
            model.Url.Should().BeEquivalentTo(defaultUrl);
        }
    }
}
