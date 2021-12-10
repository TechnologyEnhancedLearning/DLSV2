namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class FaqsServiceTests
    {
        private IFaqsDataService faqDataService = null!;
        private FaqsService faqsService = null!;

        [SetUp]
        public void Setup()
        {
            faqDataService = A.Fake<IFaqsDataService>();
            faqsService = new FaqsService(faqDataService);
        }

        [Test]
        public void GetPublishedFaqByIdForTargetGroup_returns_expected_faq_from_data_service()
        {
            // Given
            const int expectedFaqId = 1;
            const int expectedTargetGroup = 0;
            var expectedFaq = Builder<Faq>.CreateNew().With(f => f.Published = true).And(f => f.TargetGroup = 0)
                .Build();

            A.CallTo(() => faqDataService.GetPublishedFaqByIdForTargetGroup(expectedFaqId, expectedTargetGroup))
                .Returns(expectedFaq);

            // When
            var result = faqsService.GetPublishedFaqByIdForTargetGroup(expectedFaqId, expectedTargetGroup);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedFaq);

                A.CallTo(() => faqDataService.GetPublishedFaqByIdForTargetGroup(expectedFaqId, expectedTargetGroup))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void GetPublishedFaqByIdForTargetGroup_returns_null_when_data_service_returns_null()
        {
            // Given
            const int faqId = 2;
            const int targetGroup = 0;

            A.CallTo(() => faqDataService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup))
                .Returns(null);

            // When
            var result = faqsService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup);

            // Then
            result.Should().BeNull();
            A.CallTo(() => faqDataService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetPublishedFaqsForTargetGroup_returns_expected_faqs_from_data_service()
        {
            // Given
            const int expectedTargetGroup = 3;
            var expectedFaqs = Builder<Faq>.CreateListOfSize(5).All().With(f => f.Published = true)
                .And(f => f.TargetGroup = 3).Build();

            A.CallTo(() => faqDataService.GetPublishedFaqsForTargetGroup(expectedTargetGroup))
                .Returns(expectedFaqs);

            // When
            var result = faqsService.GetPublishedFaqsForTargetGroup(expectedTargetGroup).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(5);
                result.Should().BeEquivalentTo(expectedFaqs);

                A.CallTo(() => faqDataService.GetPublishedFaqsForTargetGroup(expectedTargetGroup))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void GetPublishedFaqsForTargetGroup_returns_empty_list_when_data_service_returns_empty_list()
        {
            // Given
            const int targetGroup = 5;

            A.CallTo(() => faqDataService.GetPublishedFaqsForTargetGroup(targetGroup))
                .Returns(new List<Faq>());

            // When
            var result = faqsService.GetPublishedFaqsForTargetGroup(targetGroup);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEmpty();

                A.CallTo(() => faqDataService.GetPublishedFaqsForTargetGroup(targetGroup))
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
