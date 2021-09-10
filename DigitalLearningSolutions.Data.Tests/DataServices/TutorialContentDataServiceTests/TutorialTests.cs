namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using System.Collections.Generic;
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

        [Test]
        public void GetTutorialsByCourse_returns_correct_tutorials()
        {
            // Given
            const int customisationId = 27240;
            var expectedTutorials = new List<int> { 9378, 9379, 9380, 9381, 9382, 9383, 9384, 9385, 9386, 9387 };

            // When
            var result = tutorialContentDataService.GetTutorialsForCourse(customisationId).ToList();

            // Then
            result.Should().BeEquivalentTo(expectedTutorials);
        }
    }
}
