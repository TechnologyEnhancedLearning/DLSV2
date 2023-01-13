namespace DigitalLearningSolutions.Data.Tests.DataServices.SelfAssessmentDataServiceTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public partial class SelfAssessmentDataServiceTests
    {
        [Test]
        public void GetCompetencyIdsForSelfAssessment_returns_expected_ids()
        {
            // Given
            var expectedIds = Enumerable.Range(1, 32).ToList();

            // When
            var result = selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(1).ToList();

            // Then
            result.Should().BeEquivalentTo(expectedIds);
        }

        [Test]
        public void GetNthCompetency_returns_first_competency()
        {
            // Given
            var expectedCompetency = SelfAssessmentHelper.CreateDefaultCompetency(
                competencyGroup: "Data, information and content",
                name:
                "I can find, use and store information that exists in different digital locations e.g. on a PC, shared drives, via the internet",
                description:
                "I can find, use and store information that exists in different digital locations e.g. on a PC, shared drives, via the internet",
                assessmentQuestions: new List<AssessmentQuestion>
                {
                    SelfAssessmentHelper.CreateDefaultAssessmentQuestion(1, "Where are you now"),
                    SelfAssessmentHelper.CreateDefaultAssessmentQuestion(2, "Where do you need to be"),
                }
            );

            // When
            var result = selfAssessmentDataService.GetNthCompetency(1, SelfAssessmentId, delegateUserId);

            // Then
            result.Should().BeEquivalentTo(expectedCompetency);
        }

        [Test]
        public void GetNthCompetency_returns_last_competency()
        {
            // Given
            var expectedCompetency = SelfAssessmentHelper.CreateDefaultCompetency(
                32,
                competencyGroup: "General questions",
                competencyGroupId: 7,
                name:
                "Taking an active role in my own learning is the most important thing that affects my digital literacy skills development",
                description:
                "Taking an active role in my own learning is the most important thing that affects my digital literacy skills development",
                assessmentQuestions: new List<AssessmentQuestion>
                {
                    SelfAssessmentHelper.CreateDefaultAssessmentQuestion(
                        3,
                        maxValueDescription: "Strongly agree",
                        minValueDescription: "Strongly disagree",
                        question: "To what extent do you agree",
                        assessmentQuestionInputTypeID: 1
                    ),
                }
            );

            // When
            var result = selfAssessmentDataService.GetNthCompetency(32, SelfAssessmentId, delegateUserId);

            // Then
            result.Should().BeEquivalentTo(expectedCompetency);
        }

        [Test]
        public void GetNthCompetency_returns_null_when_reached_end_of_assessment()
        {
            // When
            var result = selfAssessmentDataService.GetNthCompetency(33, SelfAssessmentId, delegateUserId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_n_zero()
        {
            // When
            var result = selfAssessmentDataService.GetNthCompetency(0, SelfAssessmentId, delegateUserId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_n_negative()
        {
            // When
            var result = selfAssessmentDataService.GetNthCompetency(-1, SelfAssessmentId, delegateUserId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_invalid_candidate()
        {
            // When
            var result = selfAssessmentDataService.GetNthCompetency(1, SelfAssessmentId, 2);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_null_when_invalid_assessment()
        {
            // When
            var result = selfAssessmentDataService.GetNthCompetency(1, 2, 2);

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
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    result + 1,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    result,
                    null
                );

                // Then
                var competency = selfAssessmentDataService.GetNthCompetency(2, SelfAssessmentId, delegateUserId);
                var actualResult = competency.AssessmentQuestions.First(question => question.Id == assessmentQuestionId)
                    .Result;
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
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    result,
                    null
                );
                var insertedResult = GetAssessmentResults(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId
                ).First();

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
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    firstResult,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    secondResult,
                    null
                );
                var insertedResults = GetAssessmentResults(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId
                ).ToList();

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
            const int invalidDelegateUserId = 2;

            using (new TransactionScope())
            {
                // When
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    invalidDelegateUserId,
                    assessmentQuestionId,
                    result,
                    null
                );
                var insertedResults = GetAssessmentResults(
                    competencyId,
                    SelfAssessmentId,
                    invalidDelegateUserId,
                    assessmentQuestionId
                );

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
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    invalidSelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    result,
                    null
                );
                var insertedResults = GetAssessmentResults(
                    competencyId,
                    invalidSelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId
                );

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
                selfAssessmentDataService.SetResultForCompetency(
                    invalidCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    result,
                    null
                );
                var insertedResults = GetAssessmentResults(
                    invalidCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId
                );

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
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    invalidAssessmentQuestionId,
                    result,
                    null
                );
                var insertedResults = GetAssessmentResults(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    invalidAssessmentQuestionId
                );

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
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    invalidResult,
                    null
                );
                var insertedResults = GetAssessmentResults(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId
                );

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
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    invalidResult,
                    null
                );
                var insertedResults = GetAssessmentResults(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId
                );

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
                selfAssessmentDataService.SetResultForCompetency(
                    firstCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    firstAssessmentQuestionId,
                    firstResult,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    firstCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    secondAssessmentQuestionId,
                    secondResult,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    secondCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    thirdAssessmentQuestionId,
                    thirdResult,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    secondCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    fourthAssessmentQuestionId,
                    fourthResult,
                    null
                );

                // Then
                var results = selfAssessmentDataService.GetMostRecentResults(SelfAssessmentId, delegateUserId).ToList();

                results.Count.Should().Be(32);
                SelfAssessmentHelper.GetQuestionResult(results, firstCompetencyId, firstAssessmentQuestionId).Should()
                    .Be(firstResult);
                SelfAssessmentHelper.GetQuestionResult(results, firstCompetencyId, secondAssessmentQuestionId).Should()
                    .Be(secondResult);
                SelfAssessmentHelper.GetQuestionResult(results, secondCompetencyId, thirdAssessmentQuestionId).Should()
                    .Be(thirdResult);
                SelfAssessmentHelper.GetQuestionResult(results, secondCompetencyId, fourthAssessmentQuestionId).Should()
                    .Be(fourthResult);
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
                selfAssessmentDataService.SetResultForCompetency(
                    firstCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    firstAssessmentQuestionId,
                    firstResult,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    firstCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    secondAssessmentQuestionId,
                    secondResult,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    secondCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    thirdAssessmentQuestionId,
                    9,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    secondCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    fourthAssessmentQuestionId,
                    9,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    secondCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    thirdAssessmentQuestionId,
                    thirdResult,
                    null
                );
                selfAssessmentDataService.SetResultForCompetency(
                    secondCompetencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    fourthAssessmentQuestionId,
                    fourthResult,
                    null
                );

                // Then
                var results = selfAssessmentDataService.GetMostRecentResults(SelfAssessmentId, delegateUserId).ToList();

                results.Count.Should().Be(32);
                SelfAssessmentHelper.GetQuestionResult(results, secondCompetencyId, thirdAssessmentQuestionId).Should()
                    .Be(thirdResult);
                SelfAssessmentHelper.GetQuestionResult(results, secondCompetencyId, fourthAssessmentQuestionId).Should()
                    .Be(fourthResult);
            }
        }

        [Test]
        public void GetCompetencyAssessmentQuestionRoleRequirements_returns_expected_results()
        {
            using (new TransactionScope())
            {
                // Given
                var expectedItem = new CompetencyAssessmentQuestionRoleRequirement
                {
                    Id = 1,
                    SelfAssessmentId = 1,
                    CompetencyId = 1,
                    AssessmentQuestionId = 1,
                    LevelValue = 1,
                    LevelRag = 1,
                };

                competencyTestHelper.InsertCompetencyAssessmentQuestionRoleRequirement(1, 1, 1, 1, 1, 1);

                // When
                var result = selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(1, 1, 1, 1);

                // Then
                result.Should().BeEquivalentTo(expectedItem);
            }
        }

        [Test]
        public void GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency_returns_expected_result()
        {
            using (new TransactionScope())
            {
                // Given
                const int competencyId = 2;
                const int assessmentQuestionId = 2;
                const int result = 5;
                selfAssessmentDataService.SetResultForCompetency(
                    competencyId,
                    SelfAssessmentId,
                    delegateUserId,
                    assessmentQuestionId,
                    result,
                    null
                );

                // When
                var results =
selfAssessmentDataService.GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
                        delegateUserId,
                        SelfAssessmentId,
                        competencyId
                    ).ToList();

                // Then
                using (new AssertionScope())
                {
                    results.Should().HaveCount(1);
                    results.First().DelegateUserId.Should().Be(delegateUserId);
                    results.First().SelfAssessmentId.Should().Be(SelfAssessmentId);
                    results.First().AssessmentQuestionId.Should().Be(assessmentQuestionId);
                    results.First().CompetencyId.Should().Be(competencyId);
                    results.First().Result.Should().Be(result);
                }
            }
        }

        private IEnumerable<int> GetAssessmentResults(
            int competencyId,
            int selfAssessmentId,
            int delegateUserId,
            int assessmentQuestionId
        )
        {
            return connection.Query<int>(
                @"SELECT Result FROM SelfAssessmentResults WHERE
                        CompetencyID = @competencyId AND
                        SelfAssessmentID = @selfAssessmentId AND
                        DelegateUserID = @delegateUserId AND
                        AssessmentQuestionID = @assessmentQuestionId",
                new { competencyId, selfAssessmentId, delegateUserId, assessmentQuestionId }
            );
        }
    }
}
