namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.New;
    using Microsoft.Extensions.Logging;

    public interface ISelfAssessmentDataService
    {
        IEnumerable<int> GetCompetencyIdsForSelfAssessment(int selfAssessmentId);

        IEnumerable<CandidateAssessment> GetCandidateAssessments(int delegateId, int selfAssessmentId);
    }

    public class SelfAssessmentDataService : ISelfAssessmentDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SelfAssessmentDataService> logger;

        public SelfAssessmentDataService(IDbConnection connection, ILogger<SelfAssessmentDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<int> GetCompetencyIdsForSelfAssessment(int selfAssessmentId)
        {
            return connection.Query<int>(
                @"SELECT
                        CompetencyID
                    FROM SelfAssessmentStructure
                    WHERE SelfAssessmentID = @selfAssessmentId",
                new { selfAssessmentId }
            );
        }

        public IEnumerable<CandidateAssessment> GetCandidateAssessments(int delegateId, int selfAssessmentId)
        {
            return connection.Query<CandidateAssessment>(
                @"SELECT
                        CandidateId AS DelegateId,
                        SelfAssessmentID,
                        CompletedDate,
                        RemovedDate
                    FROM CandidateAssessments
                    WHERE SelfAssessmentID = @selfAssessmentId
                        AND CandidateId = @delegateId",
                new { selfAssessmentId, delegateId }
            );
        }

        public IEnumerable<AssessmentQuestion> GetAssessmentQuestionsByCompetencyId(int competencyId)
        {
            return connection.Query<AssessmentQuestion>(
                @"SELECT
                        aq.ID,
                        aq.Question,
                        aq.MaxValueDescription,
                        aq.MinValueDescription,
                        aq.AssessmentQuestionInputTypeID,
                        aq.IncludeComments,
                        aq.MinValue,
                        aq.MaxValue,
                        aq.ScoringInstructions,
                        aq.AddedByAdminId,
                        aq.CommentsPrompt,
                        aq.CommentsHint
                    FROM CompetencyAssessmentQuestion AS caq
                    JOIN AssessmentQuestions AS aq ON aq.ID = caq.AssessmentQuestionID
                    WHERE caq.CompetencyID = @competencyId",
                new { competencyId }
            );
        }

        public IEnumerable<SelfAssessmentResult> GetSelfAssessmentResultsForDelegateSelfAssessmentCompetencyQuestion(
            int delegateId,
            int selfAssessmentId,
            int competencyId,
            int assessmentQuestionId
        )
        {
            return connection.Query<SelfAssessmentResult>(
                @"SELECT
                        aq.ID,
                        aq.Question,
                        aq.MaxValueDescription,
                        aq.MinValueDescription,
                        aq.AssessmentQuestionInputTypeID,
                        aq.IncludeComments,
                        aq.MinValue,
                        aq.MaxValue,
                        aq.ScoringInstructions,
                        aq.AddedByAdminId,
                        aq.CommentsPrompt,
                        aq.CommentsHint
                    FROM SelfAssessmentResults
                    WHERE CandidateID = @delegateId
                        AND SelfAssessmentID = @selfAssessmentId
                        AND CompetencyID = @competencyId
                        AND AssessmentQuestionID = @assessmentQuestionId",
                new { delegateId, selfAssessmentId, competencyId, assessmentQuestionId }
            );
        }
    }
}
