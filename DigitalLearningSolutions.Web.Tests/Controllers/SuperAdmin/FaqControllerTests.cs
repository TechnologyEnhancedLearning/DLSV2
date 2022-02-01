namespace DigitalLearningSolutions.Web.Tests.Controllers.SuperAdmin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.SuperAdmin;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class FaqControllerTests
    {
        private readonly IEnumerable<Faq> faqs = Builder<Faq>.CreateListOfSize(12)
            .TheFirst(1).With(f => f.Weighting = 40)
            .TheNext(1).With(f => f.CreatedDate = DateTime.Now.AddYears(2))
            .TheFirst(1).With(f => f.TargetGroup = 0)
            .TheRest().With(f => f.TargetGroup = 2).Build();

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
        public void Faqs_are_returned_ordered_by_most_recent_first()
        {
           //When
            var results = controller.Index();

            //Then
            results.Should().BeViewResult().WithViewName("SuperAdminFaqs")
                .Model.Should().BeOfType<FaqsPageViewModel>()
                .Subject.Faqs.First().Should().Match<SearchableFaqViewModel>(f => f.FaqId == 2);
        }
    }
}
