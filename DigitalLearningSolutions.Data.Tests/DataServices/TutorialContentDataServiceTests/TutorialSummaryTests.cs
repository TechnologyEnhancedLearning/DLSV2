namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        [Test]
        public void GetPublicTutorialSummariesByBrandId_should_return_correct_data()
        {
            // Given
            const int brandId = 1;
            var expectedIndexes = new[] { 551, 3549, 3564, 4674 };

            // When
            var result = tutorialContentDataService.GetPublicTutorialSummariesByBrandId(brandId)
                .Select(t => t.TutorialId);

            // Then
            result.Should().BeEquivalentTo(expectedIndexes);
        }
    }
}
