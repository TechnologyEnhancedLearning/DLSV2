namespace DigitalLearningSolutions.Web.Tests.Controllers.Support
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Support;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Support.Faqs;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class FaqControllerTests
    {
        private readonly IEnumerable<Faq> faqs = new List<Faq>
        {
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(2, weighting: 95, aHtml: "word documents", qText: "doc help"),
            FaqTestHelper.GetDefaultFaq(3, weighting: 75, aHtml: "excel documents", qText: "spreadsheets"),
        };

        private IConfiguration configuration = null!;
        private IFaqsService faqsService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;

        [SetUp]
        public void Setup()
        {
            configuration = A.Fake<IConfiguration>();
            faqsService = A.Fake<IFaqsService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            A.CallTo(() => faqsService.GetPublishedFaqsForTargetGroup(0))
                .Returns(faqs);
            A.CallTo(() => faqsService.GetPublishedFaqByIdForTargetGroup(2, 0))
                .Returns(faqs.ElementAt(1));
            A.CallTo(() => faqsService.GetPublishedFaqByIdForTargetGroup(5, 0))
                .Returns(null);
        }

        [Test]
        public void Faqs_list_page_should_return_expected_Faqs_view()
        {
            // Given
            var controller = new FaqsController(configuration, faqsService, searchSortFilterPaginateService);

            // When
            var result = controller.Index(DlsSubApplication.TrackingSystem);

            // Then
            result.Should().BeViewResult().WithViewName("Faqs");
        }

        [Test]
        public void AllFaqs_list_page_should_return_expected_AllFaqItems_view()
        {
            // Given
            var controller = new FaqsController(configuration, faqsService, searchSortFilterPaginateService);

            // When
            var result = controller.AllFaqItems(DlsSubApplication.TrackingSystem);

            // Then
            result.Should().BeViewResult().WithDefaultViewName("AllFaqItems");
        }

        [Test]
        public void ViewFaq_page_should_return_expected_ViewFaq_view_with_correct_faq()
        {
            // Given
            var controller = new FaqsController(configuration, faqsService, searchSortFilterPaginateService);

            // When
            var result = controller.ViewFaq(DlsSubApplication.TrackingSystem, 2);

            // Then
            result.Should().BeViewResult().WithDefaultViewName("ViewFaq")
                .Model.Should().BeOfType<SearchableFaqViewModel>()
                .Subject.Faq.FaqId.Should().Be(2);
        }

        [Test]
        public void ViewFaq_page_should_return_not_found_for_invalid_faq_item_view_request()
        {
            // Given
            var controller = new FaqsController(configuration, faqsService, searchSortFilterPaginateService);

            // When
            var result = controller.ViewFaq(DlsSubApplication.TrackingSystem, 5);

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
