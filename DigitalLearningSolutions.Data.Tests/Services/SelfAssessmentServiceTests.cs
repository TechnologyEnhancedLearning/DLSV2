namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;

    public class SelfAssessmentServiceTests
    {
        private SelfAssessmentService selfAssessmentService;
        private const int SelfAssessmentId = 1;
        private const int CandidateId = 254480;
        private SqlConnection connection;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SelfAssessmentService>>();
            selfAssessmentService = new SelfAssessmentService(connection, logger);
        }

        [Test]
        public void GetSelfAssessmentForCandidate_should_return_a_self_assessment()
        {
            // When
            var result = selfAssessmentService.GetSelfAssessmentForCandidate(CandidateId);

            // Then
            var expectedSelfAssessment = SelfAssessmentHelper.SelfAssessment(
                SelfAssessmentId,
                "Digital Capability Self Assessment",
                "When thinking about your current role, for each of the following statements rate your current confidence level (Where are you now) and where your confidence leve ought to be to undertake your role successfully (Where do you need to be). Once you have submitted your ratings they will be used to recommend useful learning resources. We will also collect data anonymously to build up a picture of digital capability across the workforce to help with service design and learning provision.",
                32
            );

            result.Should().BeEquivalentTo(expectedSelfAssessment);
        }

        [Test]
        public void GetSelfAssessmentForCandidate_should_return_null_when_there_are_no_matching_assessments()
        {
            // When
            var result = selfAssessmentService.GetSelfAssessmentForCandidate(2);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetNthCompetency_returns_first_competency()
        {
            // Given
            var expectedCompetency = SelfAssessmentHelper.Competency(
                competencyGroup: "Data, information and content",
                description: "I can find, use and store information that exists in different digital locations e.g. on a PC, shared drives, via the internet",
                assessmentQuestions: new List<AssessmentQuestion>()
                {
                    SelfAssessmentHelper.AssessmentQuestion(id: 1, question: "Where are you now"),
                    SelfAssessmentHelper.AssessmentQuestion(id: 2, question: "Where do you need to be")
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
            var expectedCompetency = SelfAssessmentHelper.Competency(
                id: 32,
                competencyGroup: "General questions",
                description: "Taking an active role in my own learning is the most important thing that affects my digital literacy skills development",
                assessmentQuestions: new List<AssessmentQuestion>()
                {
                    SelfAssessmentHelper.AssessmentQuestion(
                        id: 3,
                        maxValueDescription: "Strongly agree",
                        minValueDescription: "Strongly disagree",
                        question: "To what extent do you agree"
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
            var result = selfAssessmentService.GetNthCompetency(1, 2, CandidateId);

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
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, result + 1);
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, result);

                //Then
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
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, result);
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
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, firstResult);
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, secondResult);
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
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, invalidCandidateId, assessmentQuestionId, result);
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
                selfAssessmentService.SetResultForCompetency(competencyId, invalidSelfAssessmentId, CandidateId, assessmentQuestionId, result);
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
                selfAssessmentService.SetResultForCompetency(invalidCompetencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, result);
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
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, invalidAssessmentQuestionId, result);
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
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, invalidResult);
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
                selfAssessmentService.SetResultForCompetency(competencyId, SelfAssessmentId, CandidateId, assessmentQuestionId, invalidResult);
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
                selfAssessmentService.SetResultForCompetency(firstCompetencyId, SelfAssessmentId, CandidateId, firstAssessmentQuestionId, firstResult);
                selfAssessmentService.SetResultForCompetency(firstCompetencyId, SelfAssessmentId, CandidateId, secondAssessmentQuestionId, secondResult);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, thirdAssessmentQuestionId, thirdResult);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, fourthAssessmentQuestionId, fourthResult);

                var expectedResults = new List<Competency>()
                    {
                        SelfAssessmentHelper.Competency(
                            id: firstCompetencyId,
                            description: "I understand and stick to guidelines and regulations when using data and information to make sure of security and confidentiality requirements",
                            competencyGroup: "Data, information and content",
                            assessmentQuestions: new List<AssessmentQuestion>()
                            {
                                SelfAssessmentHelper.AssessmentQuestion(id: firstAssessmentQuestionId, question: "Where are you now", result: firstResult),
                                SelfAssessmentHelper.AssessmentQuestion(id: secondAssessmentQuestionId, question: "Where do you need to be", result: secondResult),
                            }
                        ),
                        SelfAssessmentHelper.Competency(
                            id: secondCompetencyId,
                            description: "I’m able to judge how credible and trustworthy sources of data and information are",
                            competencyGroup: "Data, information and content",
                            assessmentQuestions: new List<AssessmentQuestion>()
                            {
                                SelfAssessmentHelper.AssessmentQuestion(id: thirdAssessmentQuestionId, question: "Where are you now", result: thirdResult),
                                SelfAssessmentHelper.AssessmentQuestion(id: fourthAssessmentQuestionId, question: "Where do you need to be", result: fourthResult),
                            }
                        )
                    };

                //Then
                var results = selfAssessmentService.GetMostRecentResults(SelfAssessmentId, CandidateId).ToList();
                results.Should().BeEquivalentTo(expectedResults);
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
                selfAssessmentService.SetResultForCompetency(firstCompetencyId, SelfAssessmentId, CandidateId, firstAssessmentQuestionId, firstResult);
                selfAssessmentService.SetResultForCompetency(firstCompetencyId, SelfAssessmentId, CandidateId, secondAssessmentQuestionId, secondResult);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, thirdAssessmentQuestionId, 9);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, fourthAssessmentQuestionId, 9);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, thirdAssessmentQuestionId, thirdResult);
                selfAssessmentService.SetResultForCompetency(secondCompetencyId, SelfAssessmentId, CandidateId, fourthAssessmentQuestionId, fourthResult);

                var expectedResults = new List<Competency>()
                    {
                        SelfAssessmentHelper.Competency(
                            id: firstCompetencyId,
                            description: "I understand and stick to guidelines and regulations when using data and information to make sure of security and confidentiality requirements",
                            competencyGroup: "Data, information and content",
                            assessmentQuestions: new List<AssessmentQuestion>()
                            {
                                SelfAssessmentHelper.AssessmentQuestion(id: firstAssessmentQuestionId, question: "Where are you now", result: firstResult),
                                SelfAssessmentHelper.AssessmentQuestion(id: secondAssessmentQuestionId, question: "Where do you need to be", result: secondResult),
                            }
                        ),
                        SelfAssessmentHelper.Competency(
                            id: secondCompetencyId,
                            description: "I’m able to judge how credible and trustworthy sources of data and information are",
                            competencyGroup: "Data, information and content",
                            assessmentQuestions: new List<AssessmentQuestion>()
                            {
                                SelfAssessmentHelper.AssessmentQuestion(id: thirdAssessmentQuestionId, question: "Where are you now", result: thirdResult),
                                SelfAssessmentHelper.AssessmentQuestion(id: fourthAssessmentQuestionId, question: "Where do you need to be", result: fourthResult),
                            }
                        )
                    };

                //Then
                var results = selfAssessmentService.GetMostRecentResults(SelfAssessmentId, CandidateId).ToList();
                results.Should().BeEquivalentTo(expectedResults);
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
    }
}
