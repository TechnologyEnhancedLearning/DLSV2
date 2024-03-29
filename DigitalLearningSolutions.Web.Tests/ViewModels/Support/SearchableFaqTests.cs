﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.Faqs;
    using FluentAssertions;
    using NUnit.Framework;

    public class SearchableFaqTests
    {
        [Test]
        public void SearchableFaq_returns_model_with_cleaned_SearchableFaqAnswer()
        {
            // Given
            var faq = FaqTestHelper.GetDefaultFaq(aHtml: "<p>Helpful content.<p>");

            // When
            var searchableFaq = new SearchableFaq(faq);

            // Then
            searchableFaq.SearchableFaqAnswer.Should().Be(" p Helpful content  p ");
        }
    }
}
