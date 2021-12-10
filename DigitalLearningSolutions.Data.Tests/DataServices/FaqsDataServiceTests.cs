namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class FaqsDataServiceTests
    {
        private SqlConnection connection = null!;
        private FaqsDataService faqsDataService = null!;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            faqsDataService = new FaqsDataService(connection);
        }

        [Test]
        public void GetPublishedFaqByIdForTargetGroup_returns_published_faq_with_correct_target_group()
        {
            // Given
            const int faqId = 112;
            const int targetGroup = 0;
            var expectedFaq = Builder<Faq>.CreateNew().With(f => f.FaqId = 112).And(f => f.TargetGroup = 0)
                .And(f => f.Published = true)
                .And(
                    f => f.AHtml =
                        "No, existing learners will access the Learning Portal using their existing <strong>Delegate ID</strong>.&nbsp;"
                ).And(f => f.QText = "Do our existing learners need to register to use the Learning Portal? ")
                .And(f => f.QAnchor = "LearningPortalRegister").And(f => f.Weighting = 20)
                .And(f => f.CreatedDate = new DateTime(2017, 5, 9)).Build();

            // When
            var result = faqsDataService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup);

            // Then
            result.Should().BeEquivalentTo(expectedFaq);
        }

        [Test]
        public void GetPublishedFaqByIdForTargetGroup_returns_null_when_faq_target_group_does_not_match()
        {
            // Given
            const int faqId = 75;
            const int targetGroup = 0;

            // When
            var result = faqsDataService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetPublishedFaqByIdForTargetGroup_returns_null_when_faq_is_not_published()
        {
            // Given
            const int faqId = 104;
            const int targetGroup = 0;

            // When
            var result = faqsDataService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetPublishedFaqByIdForTargetGroup_returns_null_when_faq_does_not_exist()
        {
            // Given
            const int faqId = 9999999;
            const int targetGroup = 0;

            // When
            var result = faqsDataService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetPublishedFaqsForTargetGroup_returns_only_published_faqs_for_target_group()
        {
            // Given
            const int targetGroup = 2;
            const int expectedGroupTwoFaqId = 33;

            // When
            var result = faqsDataService.GetPublishedFaqsForTargetGroup(targetGroup).ToList();

            // Then
            result.Should().OnlyContain(f => f.TargetGroup == targetGroup && f.Published);
            result.Should().Contain(f => f.FaqId == expectedGroupTwoFaqId);
        }

        [Test]
        public void GetPublishedFaqsForTargetGroup_returns_empty_list_when_no_faqs_for_target_group()
        {
            // Given
            const int targetGroup = 15;

            // When
            var result = faqsDataService.GetPublishedFaqsForTargetGroup(targetGroup);

            // Then
            result.Should().BeEmpty();
        }
    }
}
