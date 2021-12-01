namespace DigitalLearningSolutions.Data.Tests.DataServices.SelfAssessmentDataServiceTests
{
    using System;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public partial class SelfAssessmentDataServiceTests
    {
        [Test]
        public void GetSelfAssessmentForCandidateById_should_return_a_self_assessment()
        {
            // Given
            var description =
                "When thinking about your current role, for each of the following statements rate your current confidence level " +
                "(Where are you now) and where your confidence leve ought to be to undertake your role successfully (Where do you need to be). " +
                "Once you have submitted your ratings they will be used to recommend useful learning resources. We will also collect data anonymously " +
                "to build up a picture of digital capability across the workforce to help with service design and learning provision.";

            // When
            var result = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId);

            // Then
            var expectedSelfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment(
                SelfAssessmentId,
                "Digital Capability Self Assessment",
                description,
                32,
                new DateTime(2020, 09, 01, 14, 10, 37, 447),
                null,
                null,
                false,
                true
            );

            result.Should().BeEquivalentTo(expectedSelfAssessment);
        }

        [Test]
        public void GetSelfAssessmentForCandidateById_should_return_null_when_there_are_no_matching_assessments()
        {
            // When
            var result = selfAssessmentDataService.GetSelfAssessmentForCandidateById(2, SelfAssessmentId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void UpdateLastAccessed_sets_last_accessed_to_current_time()
        {
            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.UpdateLastAccessed(SelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.LastAccessed.Should().NotBeNull();
                updatedSelfAssessment.LastAccessed.Should().BeCloseTo(DateTime.UtcNow, 1000);
            }
        }

        [Test]
        public void UpdateLastAccessed_does_not_update_invalid_self_assessment()
        {
            // Given
            const int invalidSelfAssessmentId = 0;

            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.UpdateLastAccessed(invalidSelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.LastAccessed.Should().BeNull();
            }
        }

        [Test]
        public void SetCompleteByDate_sets_complete_by_date()
        {
            // Given
            var expectedCompleteByDate = new DateTime(2020, 1, 1);

            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.SetCompleteByDate(SelfAssessmentId, CandidateId, expectedCompleteByDate);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.CompleteByDate.Should().Be(expectedCompleteByDate);
            }
        }

        [Test]
        public void SetCompleteByDate_resets_complete_by_date()
        {
            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.SetCompleteByDate(SelfAssessmentId, CandidateId, null);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.CompleteByDate.Should().BeNull();
            }
        }

        [Test]
        public void SetCompleteBy_does_not_update_invalid_self_assessment()
        {
            // Given
            const int invalidSelfAssessmentId = 2;

            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.SetCompleteByDate(invalidSelfAssessmentId, CandidateId, DateTime.UtcNow);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.CompleteByDate.Should().BeNull();
            }
        }

        [Test]
        public void SetUpdatedFlag_sets_updated_flag_to_true()
        {
            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.SetUpdatedFlag(SelfAssessmentId, CandidateId, true);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.UnprocessedUpdates.Should().BeTrue();
            }
        }

        [Test]
        public void SetUpdatedFlag_does_not_update_invalid_self_assessment()
        {
            // Given
            const int invalidSelfAssessmentId = 0;

            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.SetUpdatedFlag(SelfAssessmentId, CandidateId, false);
                selfAssessmentDataService.SetUpdatedFlag(invalidSelfAssessmentId, CandidateId, true);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.UnprocessedUpdates.Should().BeFalse();
            }
        }
        [Test]
        public void SetBookmark_sets_bookmark()
        {
            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.SetBookmark(SelfAssessmentId, CandidateId, "");
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.UserBookmark.Should().Be("");
            }
        }

        [Test]
        public void SetBookmark_does_not_set_bookmark_for_invalid_self_assessment()
        {
            // Given
            const int invalidSelfAssessmentId = 0;

            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.SetBookmark(invalidSelfAssessmentId, CandidateId, "test");
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.UserBookmark.Should().NotBe("test");
            }
        }

        [Test]
        public void IncrementLaunchCount_increases_launch_count_by_one()
        {
            using (new TransactionScope())
            {
                // When
                var originalSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;
                int originalLaunchCount = originalSelfAssessment.LaunchCount;
                selfAssessmentDataService.IncrementLaunchCount(SelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.LaunchCount.Should().BeGreaterThan(originalLaunchCount);
            }
        }

        [Test]
        public void IncrementLaunchCount_does_not_increase_launch_count_by_one_for_invalid_self_assessment()
        {
            // Given
            const int invalidSelfAssessmentId = 0;

            using (new TransactionScope())
            {
                // When
                var originalSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;
                int originalLaunchCount = originalSelfAssessment.LaunchCount;
                selfAssessmentDataService.IncrementLaunchCount(invalidSelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.LaunchCount.Should().Be(originalLaunchCount);
            }
        }
        [Test]
        public void SetSubmittedDate_sets_submitted_date_for_candidate_assessment()
        {
            using (new TransactionScope())
            {
                // When
                var originalSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;
                var originalSubmittedDate = originalSelfAssessment.SubmittedDate;
                selfAssessmentDataService.SetSubmittedDateNow(SelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                originalSubmittedDate.Should().BeNull();
                updatedSelfAssessment.SubmittedDate.Should().BeSameDateAs(DateTime.UtcNow);
            }
        }
        [Test]
        public void SetSubmittedDate_does_not_set_submitted_date_for_invalid_self_assessment()
        {
            // Given
            const int invalidSelfAssessmentId = 0;

            using (new TransactionScope())
            {
                // When
                var originalSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;
                var originalSubmittedDate = originalSelfAssessment.SubmittedDate;
                selfAssessmentDataService.SetSubmittedDateNow(invalidSelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.SubmittedDate.Should().Be(originalSubmittedDate);
            }
        }
    }
}
