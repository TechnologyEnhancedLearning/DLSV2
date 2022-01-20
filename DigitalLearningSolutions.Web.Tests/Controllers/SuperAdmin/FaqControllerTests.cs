namespace DigitalLearningSolutions.Web.Tests.Controllers.SuperAdmin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.SuperAdmin;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs;
    using DocumentFormat.OpenXml.Office2010.Excel;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class FaqControllerTests
    {
        private readonly IEnumerable<Faq> faqs = Builder<Faq>.CreateListOfSize(12)
            .All().With(f => f.Published = true)
            .TheFirst(1).With(f => f.FaqId = 1).With(f => f.Weighting = 40).With(f => f.CreatedDate = DateTime.Parse("2021, 01, 03"))
            .TheNext(1).With(f => f.FaqId = 2).With(f => f.CreatedDate = DateTime.Parse("2022, 01, 21"))
            .TheNext(1).With(f => f.CreatedDate = DateTime.Parse("2022, 01, 10"))
            .TheFirst(5).With(f => f.TargetGroup = 0)
            .TheRest().With(f => f.TargetGroup = 3).Build();

        private FaqsController controller = null!;

        private IFaqsService faqService = null!;

        [SetUp]
        public void Setup()
        {

            faqService = A.Fake<IFaqsService>();
            controller = new FaqsController(faqService);
            A.CallTo(() => faqService.GetAllFaqs())
                .Returns(faqs);
        }

        [Test]
        public void Faqs_page_should_return_expected_Faqs_view_page()
        {

            //When
            var results = controller.Index();

            //Then
            results.Should().BeViewResult().WithViewName("SuperAdminFaqs");
        }

        [Test]
        public void Faqs_page_returns_expected_results()
        {
            //Given
             A.CallTo(() => faqService.GetAllFaqs())
                 .Returns(faqs.Take(1));

            //When
            var results = controller.Index();

            //Then
            results.Should().BeViewResult().WithViewName("SuperAdminFaqs")
                .Model.Should().BeOfType<FaqsPageViewModel>()
                .Subject.Faqs.First().Should().Match<SearchableFaqViewModel>(f => f.FaqId == 1 && f.Weighting == 40);
        }

        [Test]
        public void Faqs_are_returned_ordered_by_most_recent_first()
        {
            //Given
            A.CallTo(() => faqService.GetAllFaqs())
                .Returns(faqs.Take(3));

            //When
            var results = controller.Index();

            //Then
            results.Should().BeViewResult().WithViewName("SuperAdminFaqs")
                .Model.Should().BeOfType<FaqsPageViewModel>()
                .Subject.Faqs.First().Should().Match<SearchableFaqViewModel>(f => f.FaqId == 2);
        }


        [Test]
        public void Faqs_page_returns_10_results_even_if_more_are_returned_by_service_method()
        {
            //When
            var results = controller.Index();

            //Then
            results.Should().BeViewResult().WithViewName("SuperAdminFaqs")
                .Model.Should().BeOfType<FaqsPageViewModel>()
                .Subject.Faqs.Count().Should().Be(10);
        }
    }
}
