namespace DigitalLearningSolutions.Web.Tests.ViewModels.Support
{
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Support.Faqs;
    using FluentAssertions;
    using NUnit.Framework;

    public class FaqViewModelTests
    {
        [Test]
        public void FaqViewModelTests_returns_model_with_cleaned_searchName()
        {
            // Given
            var faq = FaqTestHelper.GetDefaultFaq();
            var expectedSearchableName = "A common question?  p Helpful content  p ";

            // When
            var faqViewModel = new SearchableFaqModel(faq);

            // Then
            faqViewModel.Should().NotBeNull();
            faqViewModel.SearchableName.Should().Be(expectedSearchableName);
        }
    }
}
