namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public interface ICompetencyLearningResourcesDataService
    {
        IEnumerable<int> GetCompetencyIdsLinkedToResource(int learningResourceReferenceId);

        IEnumerable<CompetencyLearningResource> GetActiveCompetencyLearningResourcesByCompetencyId(int competencyId);

        IEnumerable<CompetencyResourceAssessmentQuestionParameter> GetCompetencyResourceAssessmentQuestionParameters(IEnumerable<int> competencyLearningResourceIds);
        int AddCompetencyLearningResource(int resourceRefID, string originalResourceName, string description, string resourceType, string link, string catalogue, decimal rating, int competencyID, int adminId);
        IEnumerable<CompetencyLearningResource> GetActiveCompetencyLearningResourcesByCompetencyIdAndReferenceId(int competencyId, int referenceId);
    }

    public class CompetencyLearningResourcesDataService : ICompetencyLearningResourcesDataService
    {
        private readonly IDbConnection connection;

        public CompetencyLearningResourcesDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<int> GetCompetencyIdsLinkedToResource(int learningResourceReferenceId)
        {
            return connection.Query<int>(
                @"SELECT
                        CompetencyID
                    FROM CompetencyLearningResources
                    WHERE LearningResourceReferenceID = @learningResourceReferenceId AND RemovedDate IS NULL",
                new { learningResourceReferenceId }
            );
        }

        public IEnumerable<CompetencyLearningResource> GetActiveCompetencyLearningResourcesByCompetencyId(int competencyId)
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
                    WHERE CompetencyID = @competencyId AND clr.RemovedDate IS NULL",
                new { competencyId }
            );
        }

        public int AddCompetencyLearningResource(int resourceRefID, string resourceName, string description, string resourceType, string link, string catalogue, decimal rating, int competencyID, int adminId)
        {
            return connection.ExecuteScalar<int>(
                @"DECLARE @learningResourceReferenceID int = (
                        SELECT TOP 1 ID
                        FROM LearningResourceReferences
                        WHERE ResourceRefID = @resourceRefID
                    );
                    IF @learningResourceReferenceID IS NULL
                    BEGIN
                        INSERT INTO LearningResourceReferences(
                            ResourceRefID,
                            OriginalResourceName,
                            OriginalDescription,
                            OriginalResourceType,
                            ResourceLink,
                            OriginalCatalogueName,
                            OriginalRating,
                            AdminID,
                            Added)
                        VALUES(
                            @resourceRefID,
                            @resourceName,
                            @description,
                            @resourceType,
                            @link,
                            @catalogue,
                            @rating,
                            @adminID,
                            GETDATE());
                        SET @learningResourceReferenceID = SCOPE_IDENTITY();
                    END
                    INSERT INTO CompetencyLearningResources(CompetencyID, LearningResourceReferenceID, AdminID)
                        VALUES (@competencyID, @learningResourceReferenceID, @adminID);
                    SELECT SCOPE_IDENTITY() AS CompetencyLearningResourceId",
                new
                {
                    resourceRefID,
                    resourceName,
                    description,
                    resourceType,
                    link,
                    catalogue,
                    rating,
                    competencyID,
                    adminID = adminId
                }
            );
        }

        public IEnumerable<CompetencyResourceAssessmentQuestionParameter> GetCompetencyResourceAssessmentQuestionParameters(IEnumerable<int> competencyLearningResourceIds)
        {
            var resourceIds = competencyLearningResourceIds?.ToArray();
            if (resourceIds == null || resourceIds.Length == 0)
            {
                return new List<CompetencyResourceAssessmentQuestionParameter>();
            }

            return connection.Query<CompetencyResourceAssessmentQuestionParameter>(
                @"SELECT
                        CompetencyLearningResourceID,
                        AssessmentQuestionID,
                        Essential,
                        RelevanceAssessmentQuestionID,
                        CompareToRoleRequirements,
                        MinResultMatch,
                        MaxResultMatch
                    FROM CompetencyResourceAssessmentQuestionParameters
                    WHERE CompetencyLearningResourceId IN @resourceIds",
                new { resourceIds }
            );
        }
        public IEnumerable<CompetencyLearningResource> GetActiveCompetencyLearningResourcesByCompetencyIdAndReferenceId(int competencyId, int referenceId)
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
             WHERE CompetencyID = @competencyId AND ResourceRefID = @referenceId AND clr.RemovedDate IS NULL",
                new { competencyId, referenceId }
            );
        }
    }
}
