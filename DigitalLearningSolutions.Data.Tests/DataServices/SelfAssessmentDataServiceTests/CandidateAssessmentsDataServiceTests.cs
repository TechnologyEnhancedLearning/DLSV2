namespace DigitalLearningSolutions.Data.Tests.DataServices.SelfAssessmentDataServiceTests
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
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

            const int DelegateUserId = 11486;
            // When
            var result = selfAssessmentDataService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId);

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
                2,
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
                selfAssessmentDataService.UpdateLastAccessed(SelfAssessmentId, delegateUserId);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
            const int DelegateUserId = 11486;

            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.UpdateLastAccessed(invalidSelfAssessmentId, DelegateUserId);

                var updatedSelfAssessment = selfAssessmentDataService.GetSelfAssessmentForCandidateById(DelegateUserId, SelfAssessmentId)!;

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
                selfAssessmentDataService.SetCompleteByDate(SelfAssessmentId, delegateUserId, expectedCompleteByDate);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                selfAssessmentDataService.SetCompleteByDate(SelfAssessmentId, delegateUserId, null);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                selfAssessmentDataService.SetCompleteByDate(invalidSelfAssessmentId, delegateUserId, DateTime.UtcNow);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                selfAssessmentDataService.SetUpdatedFlag(SelfAssessmentId, delegateUserId, true);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                selfAssessmentDataService.SetUpdatedFlag(SelfAssessmentId, delegateUserId, false);
                selfAssessmentDataService.SetUpdatedFlag(invalidSelfAssessmentId, delegateUserId, true);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                selfAssessmentDataService.SetBookmark(SelfAssessmentId, delegateUserId, "");
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                selfAssessmentDataService.SetBookmark(invalidSelfAssessmentId, delegateUserId, "test");
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                var originalSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;
                var originalLaunchCount = originalSelfAssessment.LaunchCount;
                selfAssessmentDataService.IncrementLaunchCount(SelfAssessmentId, delegateUserId);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                var originalSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;
                var originalLaunchCount = originalSelfAssessment.LaunchCount;
                selfAssessmentDataService.IncrementLaunchCount(invalidSelfAssessmentId, delegateUserId);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                var originalSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;
                var originalSubmittedDate = originalSelfAssessment.SubmittedDate;
                selfAssessmentDataService.SetSubmittedDateNow(SelfAssessmentId, delegateUserId);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

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
                var originalSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;
                var originalSubmittedDate = originalSelfAssessment.SubmittedDate;
                selfAssessmentDataService.SetSubmittedDateNow(invalidSelfAssessmentId, delegateUserId);
                var updatedSelfAssessment =
                    selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.SubmittedDate.Should().Be(originalSubmittedDate);
            }
        }

        [Test]
        public void GetCandidateAssessments_returns_expected_results()
        {
            // Given
            var expectedCandidateAssessment = new CandidateAssessment
            {
                Id = 2,
                DelegateUserID = delegateUserId,
                SelfAssessmentId = SelfAssessmentId,
                CompletedDate = null,
                RemovedDate = null,
            };

            // When
            var result = selfAssessmentDataService.GetCandidateAssessments(delegateUserId, SelfAssessmentId).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().Should().BeEquivalentTo(expectedCandidateAssessment);
            }
        }
    }
}
