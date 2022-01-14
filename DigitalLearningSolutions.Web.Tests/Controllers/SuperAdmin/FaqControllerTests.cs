namespace DigitalLearningSolutions.Web.Tests.Controllers.SuperAdmin
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.SuperAdmin;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class FaqControllerTests
    {
        private readonly IEnumerable<Faq> faqs = new List<Faq>
        {
            FaqTestHelper.GetDefaultFaq(1, weighting: 40),
            FaqTestHelper.GetDefaultFaq(2, weighting: 95, aHtml: "word documents", qText: "doc help"),
            FaqTestHelper.GetDefaultFaq(3, weighting: 75, aHtml: "excel documents", qText: "spreadsheets"),
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(),
        };

        private IFaqsService faqService = null!;

        [SetUp]
        public void Setup()
        {
            faqService = A.Fake<IFaqsService>();
            A.CallTo(() => faqService.GetAllFaqs())
                .Returns(faqs);
        }

        [Test]
        public void Faqs_page_should_return_expected_Faqs_view_page()
        {
            //Given
            var controller = new FaqsController(faqService);

            //When
            var results = controller.AllFaqItems();

            //Then
            results.Should().BeViewResult().WithViewName("SuperAdminFaqs");
        }

        [Test]
        public void Faqs_page_returns_expected_results()
        {
            //Given
            var controller = new FaqsController(faqService);
            A.CallTo(() => faqService.GetAllFaqs())
                .Returns(faqs.Take(2));

            //When
            var results = controller.AllFaqItems();

            //Then
            results.Should().BeViewResult().WithViewName("SuperAdminFaqs")
                .Model.Should().BeOfType<FaqsPageViewModel>()
                .Subject.Faqs.First().FaqId.Should().Be(1);

            results.Should().BeViewResult().WithViewName("SuperAdminFaqs")
                .Model.Should().BeOfType<FaqsPageViewModel>()
                .Subject.Faqs.First().Weighting.Should().Be(40);
        }


        [Test]
        public void Faqs_page_returns_10_results_even_if_more_are_returned_by_service_method()
        {
            //Given
            var controller = new FaqsController(faqService);

            //When
            var results = controller.AllFaqItems();

            //Then
            results.Should().BeViewResult().WithViewName("SuperAdminFaqs")
                .Model.Should().BeOfType<FaqsPageViewModel>()
                .Subject.Faqs.Count().Should().Be(10);
        }
    }
}
