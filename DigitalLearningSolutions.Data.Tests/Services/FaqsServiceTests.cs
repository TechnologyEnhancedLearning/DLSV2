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

            A.CallTo(() => faqDataService.GetFaqById(expectedFaqId))
                .Returns(expectedFaq);

            // When
            var result = faqsService.GetPublishedFaqByIdForTargetGroup(expectedFaqId, expectedTargetGroup);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedFaq);

                A.CallTo(() => faqDataService.GetFaqById(expectedFaqId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void GetPublishedFaqByIdForTargetGroup_returns_null_when_data_service_returns_null()
        {
            // Given
            const int faqId = 2;
            const int targetGroup = 0;

            A.CallTo(() => faqDataService.GetFaqById(faqId))
                .Returns(null);

            // When
            var result = faqsService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup);

            // Then
            result.Should().BeNull();
            A.CallTo(() => faqDataService.GetFaqById(faqId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetPublishedFaqByIdForTargetGroup_returns_null_when_data_service_returns_unpublished_faq()
        {
            // Given
            const int faqId = 2;
            const int targetGroup = 0;
            var faq = new Faq { Published = false, TargetGroup = 0 };

            A.CallTo(() => faqDataService.GetFaqById(faqId))
                .Returns(faq);

            // When
            var result = faqsService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup);

            // Then
            result.Should().BeNull();
            A.CallTo(() => faqDataService.GetFaqById(faqId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void
            GetPublishedFaqByIdForTargetGroup_returns_null_when_data_service_returns_faq_with_different_targetGroup()
        {
            // Given
            const int faqId = 2;
            const int targetGroup = 0;
            var faq = new Faq { Published = true, TargetGroup = 2 };

            A.CallTo(() => faqDataService.GetFaqById(faqId))
                .Returns(faq);

            // When
            var result = faqsService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup);

            // Then
            result.Should().BeNull();
            A.CallTo(() => faqDataService.GetFaqById(faqId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetPublishedFaqsForTargetGroup_returns_expected_faqs_from_data_service()
        {
            // Given
            const int expectedTargetGroup = 3;
            var expectedFaqs = Builder<Faq>.CreateListOfSize(5).All().With(f => f.Published = true)
                .And(f => f.TargetGroup = 3).Build();
            var unexpectedFaqs = Builder<Faq>.CreateListOfSize(5).All()
                .With(f => f.TargetGroup != 3).Build();
            var dataServiceFaqs = expectedFaqs.Concat(unexpectedFaqs);

            A.CallTo(() => faqDataService.GetAllFaqs())
                .Returns(dataServiceFaqs);

            // When
            var result = faqsService.GetPublishedFaqsForTargetGroup(expectedTargetGroup).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(5);
                result.Should().BeEquivalentTo(expectedFaqs);

                A.CallTo(() => faqDataService.GetAllFaqs())
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void GetPublishedFaqsForTargetGroup_returns_empty_list_when_data_service_returns_empty_list()
        {
            // Given
            const int targetGroup = 5;

            A.CallTo(() => faqDataService.GetAllFaqs())
                .Returns(new List<Faq>());

            // When
            var result = faqsService.GetPublishedFaqsForTargetGroup(targetGroup);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEmpty();

                A.CallTo(() => faqDataService.GetAllFaqs())
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void GetAllFaqs_calls_data_service_method_and_returns_expected_results()
        {
            //Given
            var expectedFaqs = Builder<Faq>.CreateListOfSize(10)
                .Build();
            A.CallTo(() => faqDataService.GetAllFaqs())
                .Returns(expectedFaqs);

            //When
            var result = faqsService.GetAllFaqs();

            //Then
            result.Should().Equal(expectedFaqs);
        }

        [Test]
        public void GetAll_returns_empty_when_data_service_returns_null()
        {
            var emptyList = Enumerable.Empty<Faq>();

            A.CallTo(() => faqDataService.GetAllFaqs())
                .Returns(emptyList);

            // When
            var result = faqsService.GetAllFaqs();

            // Then
            result.Should().BeEmpty();
            A.CallTo(() => faqDataService.GetAllFaqs())
                .MustHaveHappenedOnceExactly();
        }
    }
}
