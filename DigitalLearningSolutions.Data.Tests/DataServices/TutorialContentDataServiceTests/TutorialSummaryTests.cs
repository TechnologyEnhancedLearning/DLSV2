namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        [Test]
        public void GetPublicTutorialSummariesByBrandId_should_return_correct_data()
        {
            // Given
            const int brandId = 1;
            var indexes = new int[] { 551, 3549, 3564, 4674 };

            // When
            var tutorial = tutorialContentDataService.GetPublicTutorialSummariesByBrandId(brandId)
                .Select(t => t.TutorialId);

            // Then
            tutorial.Should().BeEquivalentTo(indexes);
        }
    }
}
