namespace DigitalLearningSolutions.Web.Tests.ViewModels.FindYourCentre
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using FluentAssertions;
    using NUnit.Framework;

    public class FindYourCentreViewModelTests
    {
        private string defaultUrl = ConfigHelper.GetAppConfig()["CurrentSystemBaseUrl"] + "/findyourcentre?nonav=true";

        [Test]
        public void FindYourCentreViewModel_without_centreid_should_have_default_Url()
        {
            // When
            var model = new FindYourCentreViewModel();

            // Then
            model.Url.Should().BeEquivalentTo(defaultUrl);
        }

        [Test]
        public void FindYourCentreViewModel_with_centreid_should_have_centreid_added_to_Url()
        {
            // Given
            var centreId = "5";

            // When
            var model = new FindYourCentreViewModel(centreId);

            // Then
            model.Url.Should().BeEquivalentTo(defaultUrl + "&centreid=" + centreId);
        }
    }
}
