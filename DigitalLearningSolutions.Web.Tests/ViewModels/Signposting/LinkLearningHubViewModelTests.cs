namespace DigitalLearningSolutions.Web.Tests.ViewModels.Signposting
{
    using DigitalLearningSolutions.Web.ViewModels.Signposting.LinkLearningHubSso;
    using FluentAssertions;
    using NUnit.Framework;

    public class LinkLearningHubViewModelTests
    {
        [Test]
        [TestCase(null, false)]
        [TestCase(1, true)]
        public void LinkLearningHubViewModel_constructor_should_set_properties_correctly(int? resourceId, bool showLink)
        {
            // When
            var model = new LinkLearningHubViewModel(resourceId);

            // Then
            model.ResourceLinkId.Should().Be(resourceId);
            model.ShowResourceLink.Should().Be(showLink);
        }
    }
}
