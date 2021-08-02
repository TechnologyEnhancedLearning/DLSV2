namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        [Test]
        public void GetTutorialsBySectionId_returns_tutorials_correctly()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;

            // When
            var result = tutorialContentDataService.GetTutorialsBySectionId(sectionId, customisationId).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(4);
                result.First().TutorialId.Should().Be(49);
                result.First().TutorialName.Should().Be("View documents");
                result.First().Status.Should().BeTrue();
                result.First().DiagStatus.Should().BeTrue();
            }
        }
    }
}
