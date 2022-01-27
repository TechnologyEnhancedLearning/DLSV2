namespace DigitalLearningSolutions.Web.Tests.ViewModels.Support
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Shared.Faqs;
    using DigitalLearningSolutions.Web.ViewModels.Support.Faqs;
    using FluentAssertions;
    using NUnit.Framework;

    public class FaqsViewModelTests
    {
        private readonly IEnumerable<Faq> allFaqs = new List<Faq>
        {
            FaqTestHelper.GetDefaultFaq(aHtml: "helpful documents"),
            FaqTestHelper.GetDefaultFaq(2, weighting: 65, aHtml: "word documents", qText: "doc help"),
            FaqTestHelper.GetDefaultFaq(3, weighting: 75, aHtml: "excel documents", qText: "spreadsheets"),
            FaqTestHelper.GetDefaultFaq(
                4,
                weighting: 67,
                aHtml: "document creator updates",
                qText: "how to update software"
            ),
            FaqTestHelper.GetDefaultFaq(
                5,
                weighting: 85,
                aHtml: "advanced word document processing",
                qText: "advanced doc help"
            ),
            FaqTestHelper.GetDefaultFaq(
                6,
                weighting: 85,
                aHtml: "first time user instructions document",
                qText: "read me first"
            ),
            FaqTestHelper.GetDefaultFaq(7, weighting: 95, aHtml: "more help documents", qText: "general help"),
            FaqTestHelper.GetDefaultFaq(8, weighting: 70, aHtml: "tips and tricks documents", qText: "cheatsheet"),
            FaqTestHelper.GetDefaultFaq(
                9,
                weighting: 62,
                aHtml: "presentations instructions document",
                qText: "help with slides"
            ),
            FaqTestHelper.GetDefaultFaq(10, weighting: 73, aHtml: "printing documents", qText: "printer help"),
            FaqTestHelper.GetDefaultFaq(
                11,
                weighting: 73,
                aHtml: "dual monitors setup documentation",
                qText: "monitor setup"
            ),
        };

        [Test]
        public void FaqsPageViewModel_search_for_document_page_one_returns_expected_first_ten_faqs()
        {
            // Given
            var faqViewModels = allFaqs.Select(f => new SearchableFaq(f));

            // When
            var result = new FaqsPageViewModel(
                DlsSubApplication.TrackingSystem,
                SupportPage.HelpDocumentation,
                "currentSystemBaseUrl",
                faqViewModels,
                1,
                "document"
            );

            // Then
            var faqs = result.Faqs.ToList();
            faqs.Should().HaveCount(10);
        }

        [Test]
        public void FaqsPageViewModel_search_for_document_page_two_returns_expected_one_faq()
        {
            // Given
            var faqViewModels = allFaqs.Select(f => new SearchableFaq(f)).ToList();
            var expectedFaq = new SearchableFaqViewModel(DlsSubApplication.TrackingSystem, faqViewModels.ElementAt(8));

            // When
            var result = new FaqsPageViewModel(
                DlsSubApplication.TrackingSystem,
                SupportPage.HelpDocumentation,
                "currentSystemBaseUrl",
                faqViewModels,
                2,
                "document"
            );

            // Then
            var faqs = result.Faqs.ToList();
            faqs.Should().HaveCount(1);
            faqs.First().Should().BeEquivalentTo(expectedFaq);
        }

        [Test]
        public void FaqsPageViewModel_search_for_help_returns_expected_six_faqs()
        {
            // Given
            var faqViewModels = allFaqs.Select(f => new SearchableFaq(f)).ToList();
            var expectedFaqIds = new List<int> { 1, 2, 5, 7, 9, 10 };

            // When
            var result = new FaqsPageViewModel(
                DlsSubApplication.TrackingSystem,
                SupportPage.HelpDocumentation,
                "currentSystemBaseUrl",
                faqViewModels,
                1,
                "help"
            );

            // Then
            var faqs = result.Faqs.ToList();
            faqs.Should().HaveCount(6);
            faqs.Should().OnlyHaveUniqueItems()
                .And.OnlyContain(f => expectedFaqIds.Contains(f.Faq.FaqId));
        }

        [Test]
        public void FaqsPageViewModel_search_for_word_returns_expected_two_faqs_in_weight_order_descending()
        {
            // Given
            var faqViewModels = allFaqs.Select(f => new SearchableFaq(f)).ToList();
            var expectedFirstFaq = new SearchableFaqViewModel(
                DlsSubApplication.TrackingSystem,
                faqViewModels.ElementAt(4)
            );
            var expectedSecondFaq = new SearchableFaqViewModel(
                DlsSubApplication.TrackingSystem,
                faqViewModels.ElementAt(1)
            );

            // When
            var result = new FaqsPageViewModel(
                DlsSubApplication.TrackingSystem,
                SupportPage.HelpDocumentation,
                "currentSystemBaseUrl",
                faqViewModels,
                1,
                "word"
            );

            // Then
            var faqs = result.Faqs.ToList();
            faqs.Should().HaveCount(2);
            faqs.ElementAt(0).Should().BeEquivalentTo(expectedFirstFaq);
            faqs.ElementAt(1).Should().BeEquivalentTo(expectedSecondFaq);
        }
    }
}
