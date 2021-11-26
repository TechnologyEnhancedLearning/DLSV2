namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
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
            const int expectedFaqId = 81;
            const int expectedTargetGroup = 0;

            // When
            var result = faqsDataService.GetPublishedFaqByIdForTargetGroup(expectedFaqId, expectedTargetGroup);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.FaqId.Should().Be(expectedFaqId);
                result.TargetGroup.Should().Be(expectedTargetGroup);
                result.Published.Should().BeTrue();
            }
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
