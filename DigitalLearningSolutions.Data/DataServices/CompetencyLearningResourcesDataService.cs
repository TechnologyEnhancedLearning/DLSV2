namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;    
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public interface ICompetencyLearningResourcesDataService
    {
        IEnumerable<int> GetCompetencyIdsLinkedToResource(int learningResourceReferenceId);

        IEnumerable<CompetencyLearningResource> GetActiveCompetencyLearningResourcesByCompetencyId(int competencyId);

        IEnumerable<CompetencyResourceAssessmentQuestionParameter> GetCompetencyResourceAssessmentQuestionParameters(IEnumerable<int> competencyLearningResourceIds);
        int AddCompetencyLearningResource(int resourceRefID, string originalResourceName, string description, string resourceType, string link, string catalogue, decimal rating, int competencyID, int adminId);
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
                @$" DECLARE @learningResourceReferenceID int
                    IF NOT EXISTS(SELECT * FROM LearningResourceReferences WHERE @resourceRefID = resourceRefID)
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
                                GETDATE())
                            SELECT @learningResourceReferenceID = SCOPE_IDENTITY()
                        END
                    ELSE
                        BEGIN
                            SELECT TOP 1 @learningResourceReferenceID = ID 
                            FROM LearningResourceReferences 
                            WHERE @resourceRefID = resourceRefID
                        END
                    INSERT INTO CompetencyLearningResources(CompetencyID, LearningResourceReferenceID, AdminID)
                           VALUES (@competencyID, @learningResourceReferenceID, @adminID)
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
                    adminId
                }
            );
        }

        public IEnumerable<CompetencyResourceAssessmentQuestionParameter>GetCompetencyResourceAssessmentQuestionParameters(IEnumerable<int> competencyLearningResourceIds)
        {
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
                    WHERE CompetencyLearningResourceId IN @competencyLearningResourceIds",
                new { competencyLearningResourceIds }
            );
        }
    }
}
