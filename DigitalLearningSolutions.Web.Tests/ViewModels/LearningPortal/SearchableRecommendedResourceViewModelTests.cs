namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning;
    using FluentAssertions;
    using NUnit.Framework;

    public class SearchableRecommendedResourceViewModelTests
    {
        [Test]
        [TestCase(120, "Essential")]
        [TestCase(100, "Essential")]
        [TestCase(99.9, "Recommended")]
        [TestCase(60, "Recommended")]
        [TestCase(40, "Recommended")]
        [TestCase(39.9, "Optional")]
        [TestCase(20, "Optional")]
        public void Recommendation_Rating_string_gets_populated_with_the_expected_value(
            decimal recommendationScore,
            string expectedRating
        )
        {
            // Given
            var recommendedResource = new RecommendedResource { RecommendationScore = recommendationScore };

            // When
            var result = new SearchableRecommendedResourceViewModel(
                recommendedResource,
                1,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            result.Rating.Should().BeEquivalentTo(expectedRating);
        }
    }
}
