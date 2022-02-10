namespace DigitalLearningSolutions.Web.Tests.ViewModels.Signposting
{
    using DigitalLearningSolutions.Web.ViewModels.Signposting;
    using FluentAssertions;
    using NUnit.Framework;

    public class LinkLearningHubViewModelTests
    {
        [Test]
        [TestCase(null, false, false)]
        [TestCase(1, true, false)]
        [TestCase(2, true, true)]
        public void LinkLearningHubViewModel_constructor_should_set_properties_correctly(int? resourceId, bool showLink, bool showAlreadyLinkedWarning)
        {
            // When
            var model = new LinkLearningHubViewModel(showAlreadyLinkedWarning, resourceId);

            // Then
            model.ResourceLinkId.Should().Be(resourceId);
            model.ShowResourceLink.Should().Be(showLink);
            model.ShowIsAlreadyLinkedWarning.Should().Be(showAlreadyLinkedWarning);
        }
    }
}
