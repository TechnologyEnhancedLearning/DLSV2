namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public interface ICompetencyLearningResourcesDataService
    {
        IEnumerable<int> GetCompetencyIdsByLearningResourceReferenceId(int learningResourceReferenceId);

        IEnumerable<CompetencyLearningResource> GetCompetencyLearningResourcesByCompetencyId(int competencyId);

        void AddCompetencyLearningResource(
            int resourceRefId,
            string originalResourceName,
            int competencyId,
            int adminId
        );

        IEnumerable<CompetencyResourceAssessmentQuestionParameter> GetCompetencyResourceAssessmentQuestionParameters(
            IEnumerable<int> competencyLearningResourceIds
        );
    }

    public class CompetencyLearningResourcesDataService : ICompetencyLearningResourcesDataService
    {
        private readonly IDbConnection connection;

        public CompetencyLearningResourcesDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<int> GetCompetencyIdsByLearningResourceReferenceId(int learningResourceReferenceId)
        {
            return connection.Query<int>(
                @"SELECT
                        CompetencyID
                    FROM CompetencyLearningResources
                    WHERE LearningResourceReferenceID = @learningResourceReferenceId",
                new { learningResourceReferenceId }
            );
        }

        public IEnumerable<CompetencyLearningResource> GetCompetencyLearningResourcesByCompetencyId(int competencyId)
        {
            return connection.Query<CompetencyLearningResource>(
                @"SELECT
                        clr.ID,
                        clr.CompetencyID,
                        clr.LearningResourceReferenceID,
                        clr.AdminID,
                        lrr.ResourceRefID AS LearningHubResourceReferenceId
                    FROM CompetencyLearningResources AS clr
                    INNER JOIN LearningResourceReferences AS lrr ON lrr.ID = clr.LearningResourceReferenceID
                    WHERE CompetencyID = @competencyId",
                new { competencyId }
            );
        }

        public void AddCompetencyLearningResource(
            int resourceRefId,
            string originalResourceName,
            int competencyId,
            int adminId
        )
        {
            connection.Execute(
                @" DECLARE @learningResourceReferenceID int
                    IF NOT EXISTS(SELECT * FROM LearningResourceReferences WHERE @resourceRefID = resourceRefID)
                        BEGIN
                            INSERT INTO LearningResourceReferences(ResourceRefID, OriginalResourceName, AdminID, Added)
                            VALUES(@resourceRefID, @originalResourceName, @adminID, GETDATE())
                            SELECT @learningResourceReferenceID = SCOPE_IDENTITY()
                        END
                    ELSE
                        BEGIN
                            SELECT TOP 1 @learningResourceReferenceID = ID 
                            FROM LearningResourceReferences 
                            WHERE @resourceRefID = resourceRefID
                        END
                    INSERT INTO CompetencyLearningResources(CompetencyID, LearningResourceReferenceID, AdminID)
                           VALUES (@competencyID, @learningResourceReferenceID, @adminID)",
                new { resourceRefID = resourceRefId, originalResourceName, competencyID = competencyId, adminId }
            );
        }

        public IEnumerable<CompetencyResourceAssessmentQuestionParameter>
            GetCompetencyResourceAssessmentQuestionParameters(IEnumerable<int> competencyLearningResourceIds)
        {
            return connection.Query<CompetencyResourceAssessmentQuestionParameter>(
                @"SELECT
                        ID,
                        CompetencyLearningResourceID,
                        AssessmentQuestionID,
                        Essential,
                        RelevanceAssessmentQuestionID,
                        CompareToRoleRequirements
                    FROM CompetencyResourceAssessmentQuestionParameters
                    WHERE CompetencyLearningResourceId IN @competencyLearningResourceIds",
                new { competencyLearningResourceIds }
            );
        }
    }
}
