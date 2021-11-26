namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;

    public class SelfAssessmentServiceTests
    {
        private SelfAssessmentService selfAssessmentService = null!;
        private ISelfAssessmentDataService selfAssessmentDataService = null!;
        private const int SelfAssessmentId = 1;
        private const int CandidateId = 11;
        private SqlConnection connection = null!;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SelfAssessmentService>>();
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();
            selfAssessmentService = new SelfAssessmentService(connection, logger, selfAssessmentDataService);
        }

        [Test]
        public void GetSelfAssessmentForCandidateById_should_return_a_self_assessment()
        {
            // When
            var result = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId);

            // Then
            var expectedSelfAssessment = SelfAssessmentHelper.CreateDefaultSelfAssessment(
                SelfAssessmentId,
                "Digital Capability Self Assessment",
                "When thinking about your current role, for each of the following statements rate your current confidence level (Where are you now) and where your confidence leve ought to be to undertake your role successfully (Where do you need to be). Once you have submitted your ratings they will be used to recommend useful learning resources. We will also collect data anonymously to build up a picture of digital capability across the workforce to help with service design and learning provision.",
                32,
                new DateTime(2020, 09, 01, 14, 10, 37, 447), null, null,
                false,
                true,
                true,
                true
            );

            result.Should().BeEquivalentTo(expectedSelfAssessment);
        }

        [Test]
        public void GetSelfAssessmentForCandidateById_should_return_null_when_there_are_no_matching_assessments()
        {
            // When
            var result = selfAssessmentService.GetSelfAssessmentForCandidateById(2, SelfAssessmentId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_first_competency()
        {
            // Given
            var expectedCompetency = SelfAssessmentHelper.CreateDefaultCompetency(
                competencyGroup: "Data, information and content",
                name: "I can find, use and store information that exists in different digital locations e.g. on a PC, shared drives, via the internet",
                description: "I can find, use and store information that exists in different digital locations e.g. on a PC, shared drives, via the internet",
                assessmentQuestions: new List<AssessmentQuestion>()
                {
                    SelfAssessmentHelper.CreateDefaultAssessmentQuestion(id: 1, question: "Where are you now"),
                    SelfAssessmentHelper.CreateDefaultAssessmentQuestion(id: 2, question: "Where do you need to be")
                }
            );

            // When
            var result = selfAssessmentService.GetNthCompetency(1, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeEquivalentTo(expectedCompetency);
        }

        [Test]
        public void GetNthCompetency_returns_last_competency()
        {
            // Given
            var expectedCompetency = SelfAssessmentHelper.CreateDefaultCompetency(
                id: 32,
                competencyGroup: "General questions",
                competencyGroupId: 7,
                name: "Taking an active role in my own learning is the most important thing that affects my digital literacy skills development",
                description: "Taking an active role in my own learning is the most important thing that affects my digital literacy skills development",
                assessmentQuestions: new List<AssessmentQuestion>()
                {
                    SelfAssessmentHelper.CreateDefaultAssessmentQuestion(
                        id: 3,
                        maxValueDescription: "Strongly agree",
                        minValueDescription: "Strongly disagree",
                        question: "To what extent do you agree",
                       assessmentQuestionInputTypeID: 1
                    )
                });

            // When
            var result = selfAssessmentService.GetNthCompetency(32, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeEquivalentTo(expectedCompetency);
        }

        [Test]
        public void GetNthCompetency_returns_null_when_reached_end_of_assessment()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(33, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_n_zero()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(0, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_n_negative()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(-1, SelfAssessmentId, CandidateId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_invalid_candidate()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(1, SelfAssessmentId, 1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_invalid_assessment()
        {
            // When
            var result = selfAssessmentService.GetNthCompetency(1, 2, 1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_gets_the_latest_result()
        {
            // Given
            const int competencyId = 2;
            const int assessmentQuestionId = 2;
            const int result = 5;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, result + 1, null);
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, result, null);

                // Then
                var competency = selfAssessmentService.GetNthCompetency(2, SelfAssessmentId, CandidateId);
                var actualResult = competency.AssessmentQuestions.First(question => question.Id == assessmentQuestionId).Result;
                result.Should().Be(actualResult);
            }
        }

        [Test]
        public void SetResultForCompetency_sets_result()
        {
            // Given
            const int competencyId = 2;
            const int assessmentQuestionId = 2;
            const int result = 5;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, result, null);
                var insertedResult = GetAssessmentResults(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId).First();

                // Then
                insertedResult.Should().Be(result);
            }
        }

        [Test]
        public void SetResultForCompetency_does_not_overwrite_previous_result()
        {
            // Given
            const int competencyId = 2;
            const int assessmentQuestionId = 2;
            const int firstResult = 5;
            const int secondResult = 10;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, firstResult, null);
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, secondResult, null);
                var insertedResults = GetAssessmentResults(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId).ToList();

                // Then
                insertedResults.Should().HaveCount(2);
                insertedResults[0].Should().Be(firstResult);
                insertedResults[1].Should().Be(secondResult);
            }
        }

        [Test]
        public void SetResultForCompetency_does_not_set_result_for_invalid_candidate()
        {
            // Given
            const int competencyId = 2;
            const int assessmentQuestionId = 2;
            const int result = 5;
            const int invalidCandidateId = 1;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, invalidCandidateId, assessmentQuestionId, result, null);
                var insertedResults = GetAssessmentResults(competencyId, SelfAssessmentId, invalidCandidateId, assessmentQuestionId);

                // Then
                insertedResults.Should().BeEmpty();
            }
        }

        [Test]
        public void SetResultForCompetency_does_not_set_result_for_invalid_assessment()
        {
            // Given
            const int competencyId = 2;
            const int assessmentQuestionId = 2;
            const int result = 5;
            const int invalidSelfAssessmentId = 2;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(competencyId, invalidSelfAssessmentId, CandidateId, assessmentQuestionId, result, null);
                var insertedResults = GetAssessmentResults(competencyId, invalidSelfAssessmentId, CandidateId, assessmentQuestionId);

                // Then
                insertedResults.Should().BeEmpty();
            }
        }

        [Test]
        public void SetResultForCompetency_does_not_set_result_for_invalid_competency()
        {
            // Given
            const int invalidCompetencyId = 33;
            const int assessmentQuestionId = 2;
            const int result = 5;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(invalidCompetencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, result, null);
                var insertedResults = GetAssessmentResults(invalidCompetencyId, SelfAssessmentId, CandidateId, assessmentQuestionId);

                // Then
                insertedResults.Should().BeEmpty();
            }
        }

        [Test]
        public void SetResultForCompetency_does_not_set_result_for_invalid_question()
        {
            // Given
            const int competencyId = 33;
            const int invalidAssessmentQuestionId = 4;
            const int result = 5;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, invalidAssessmentQuestionId, result, null);
                var insertedResults = GetAssessmentResults(competencyId, SelfAssessmentId, CandidateId, invalidAssessmentQuestionId);

                // Then
                insertedResults.Should().BeEmpty();
            }
        }

        [Test]
        public void SetResultForCompetency_does_not_set_result_for_negative_result()
        {
            // Given
            const int competencyId = 33;
            const int assessmentQuestionId = 4;
            const int invalidResult = -1;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, invalidResult, null);
                var insertedResults = GetAssessmentResults(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId);

                // Then
                insertedResults.Should().BeEmpty();
            }
        }

        [Test]
        public void SetResultForCompetency_does_not_set_result_for_invalid_result()
        {
            // Given
            const int competencyId = 33;
            const int assessmentQuestionId = 4;
            const int invalidResult = 11;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, invalidResult, null);
                var insertedResults = GetAssessmentResults(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId);

                // Then
                insertedResults.Should().BeEmpty();
            }
        }

        [Test]
        public void GetMostRecentResults_gets_multiple_competencies()
        {
            // Given
            const int firstCompetencyId = 2;
            const int secondCompetencyId = 3;
            const int firstAssessmentQuestionId = 1;
            const int secondAssessmentQuestionId = 2;
            const int thirdAssessmentQuestionId = 1;
            const int fourthAssessmentQuestionId = 2;
            const int firstResult = 1;
            const int secondResult = 2;
            const int thirdResult = 3;
            const int fourthResult = 4;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(firstCompetencyId, SelfAssessmentId, CandidateId, firstAssessmentQuestionId, firstResult, null);
                selfAssessmentService.SetResultForCompetency(firstCompetencyId, SelfAssessmentId, CandidateId, secondAssessmentQuestionId, secondResult, null);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, thirdAssessmentQuestionId, thirdResult, null);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, fourthAssessmentQuestionId, fourthResult, null);

                // Then
                var results = selfAssessmentService.GetMostRecentResults(SelfAssessmentId, CandidateId).ToList();

                results.Count.Should().Be(32);
                SelfAssessmentHelper.GetQuestionResult(results, firstCompetencyId, firstAssessmentQuestionId).Should().Be(firstResult);
                SelfAssessmentHelper.GetQuestionResult(results, firstCompetencyId, secondAssessmentQuestionId).Should().Be(secondResult);
                SelfAssessmentHelper.GetQuestionResult(results, secondCompetencyId, thirdAssessmentQuestionId).Should().Be(thirdResult);
                SelfAssessmentHelper.GetQuestionResult(results, secondCompetencyId, fourthAssessmentQuestionId).Should().Be(fourthResult);
                SelfAssessmentHelper.GetQuestionResult(results, 4, 1).Should().BeNull();
            }
        }

        [Test]
        public void GetMostRecentResults_gets_most_recent_results()
        {
            // Given
            const int firstCompetencyId = 2;
            const int secondCompetencyId = 3;
            const int firstAssessmentQuestionId = 1;
            const int secondAssessmentQuestionId = 2;
            const int thirdAssessmentQuestionId = 1;
            const int fourthAssessmentQuestionId = 2;
            const int firstResult = 1;
            const int secondResult = 2;
            const int thirdResult = 3;
            const int fourthResult = 4;

            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetResultForCompetency(firstCompetencyId, SelfAssessmentId, CandidateId, firstAssessmentQuestionId, firstResult, null);
                selfAssessmentService.SetResultForCompetency(firstCompetencyId, SelfAssessmentId, CandidateId, secondAssessmentQuestionId, secondResult, null);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, thirdAssessmentQuestionId, 9, null);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, fourthAssessmentQuestionId, 9, null);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, thirdAssessmentQuestionId, thirdResult, null);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, fourthAssessmentQuestionId, fourthResult, null);

                // Then
                var results = selfAssessmentService.GetMostRecentResults(SelfAssessmentId, CandidateId).ToList();

                results.Count.Should().Be(32);
                SelfAssessmentHelper.GetQuestionResult(results, secondCompetencyId, thirdAssessmentQuestionId).Should().Be(thirdResult);
                SelfAssessmentHelper.GetQuestionResult(results, secondCompetencyId, fourthAssessmentQuestionId).Should().Be(fourthResult);
            }
        }

        [Test]
        public void UpdateLastAccessed_sets_last_accessed_to_current_time()
        {
            using (new TransactionScope())
            {
                // When
                selfAssessmentService.UpdateLastAccessed(SelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                selfAssessmentService.UpdateLastAccessed(invalidSelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                selfAssessmentService.SetCompleteByDate(SelfAssessmentId, CandidateId, expectedCompleteByDate);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                selfAssessmentService.SetCompleteByDate(SelfAssessmentId, CandidateId, null);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                selfAssessmentService.SetCompleteByDate(invalidSelfAssessmentId, CandidateId, DateTime.UtcNow);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.CompleteByDate.Should().BeNull();
            }
        }

        private IEnumerable<int> GetAssessmentResults(int competencyId, int selfAssessmentId, int candidateId, int assessmentQuestionId)
        {
            return connection.Query<int>(
                @"SELECT Result FROM SelfAssessmentResults WHERE
                        CompetencyID = @competencyId AND
                        SelfAssessmentID = @selfAssessmentId AND
                        CandidateID = @candidateId AND
                        AssessmentQuestionID = @assessmentQuestionId",
                new { competencyId, selfAssessmentId, candidateId, assessmentQuestionId }
            );
        }
        [Test]
        public void SetUpdatedFlag_sets_updated_flag_to_true()
        {
            using (new TransactionScope())
            {
                // When
                selfAssessmentService.SetUpdatedFlag(SelfAssessmentId, CandidateId, true);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                selfAssessmentService.SetUpdatedFlag(SelfAssessmentId, CandidateId, false);
                selfAssessmentService.SetUpdatedFlag(invalidSelfAssessmentId, CandidateId, true);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                selfAssessmentService.SetBookmark(SelfAssessmentId, CandidateId, "");
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                selfAssessmentService.SetBookmark(invalidSelfAssessmentId, CandidateId, "test");
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                var originalSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;
                int originalLaunchCount = originalSelfAssessment.LaunchCount;
                selfAssessmentService.IncrementLaunchCount(SelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                var originalSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;
                int originalLaunchCount = originalSelfAssessment.LaunchCount;
                selfAssessmentService.IncrementLaunchCount(invalidSelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

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
                var originalSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;
                var originalSubmittedDate = originalSelfAssessment.SubmittedDate;
                selfAssessmentService.SetSubmittedDateNow(SelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                originalSubmittedDate.Should().BeNull();
                    updatedSelfAssessment.SubmittedDate.Should().BeSameDateAs(System.DateTime.UtcNow);
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
                var originalSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;
                var originalSubmittedDate = originalSelfAssessment.SubmittedDate;
                selfAssessmentService.SetSubmittedDateNow(invalidSelfAssessmentId, CandidateId);
                var updatedSelfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(CandidateId, SelfAssessmentId)!;

                // Then
                updatedSelfAssessment.SubmittedDate.Should().Be(originalSubmittedDate);
            }
        }
    }
}
