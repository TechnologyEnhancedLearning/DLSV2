namespace DigitalLearningSolutions.Web.Tests.ViewModels.Support
{
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Support.Faqs;
    using FluentAssertions;
    using NUnit.Framework;

    public class SearchableFaqTests
    {
        [Test]
        public void SearchableFaq_returns_model_with_cleaned_SearchableFaqAnswer()
        {
            // Given
            var faq = FaqTestHelper.GetDefaultFaq();
            var expectedSearchableFaqAnswer = " p Helpful content  p ";

            // When
            var searchableFaq = new SearchableFaq(faq);

            // Then
            searchableFaq.SearchableFaqAnswer.Should().Be(expectedSearchableFaqAnswer);
        }
    }
}
