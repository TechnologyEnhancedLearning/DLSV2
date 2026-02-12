namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    public interface ICompetencyAssessmentDataService
    {
        //GET DATA
        IEnumerable<CompetencyAssessment> GetAllCompetencyAssessments(int adminId);

        IEnumerable<CompetencyAssessment> GetCompetencyAssessmentsForAdminId(int adminId);

        CompetencyAssessmentBase? GetCompetencyAssessmentBaseById(int competencyAssessmentId, int adminId);

        CompetencyAssessmentBase? GetCompetencyAssessmentBaseByName(string competencyAssessmentName, int adminId);
        CompetencyAssessment? GetCompetencyAssessmentById(int competencyAssessmentId, int adminId);
        IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups();
        IEnumerable<NRPSubGroups> GetNRPSubGroups(int? nRPProfessionalGroupID);
        IEnumerable<NRPRoles> GetNRPRoles(int? nRPSubGroupID);

        CompetencyAssessmentTaskStatus GetOrInsertAndReturnAssessmentTaskStatus(int assessmentId, bool frameworkBased);

        int[] GetLinkedFrameworkIds(int assessmentId);

        int? GetPrimaryLinkedFrameworkId(int assessmentId);

        int GetCompetencyCountByFrameworkId(int assessmentId, int frameworkId);

        IEnumerable<Competency> GetCompetenciesForCompetencyAssessment(int competencyAssessmentId);
        IEnumerable<LinkedFramework> GetLinkedFrameworksForCompetencyAssessment(int competencyAssessmentId);
        int[] GetLinkedFrameworkCompetencyIds(int competencyAssessmentId, int frameworkId);
        CompetencyAssessmentFeatures? GetCompetencyAssessmentFeaturesTaskStatus(int competencyAssessmentId);
        int? GetSelfAssessmentStructure(int competencyAssessmentId);
        IEnumerable<CompetencyWithAssessmentQuestionRoleRequirements> GetCompetencyWithAssessmentQuestionRoleRequirements(int competencyAssessmentId, int? competencyId, int? assessmentQuestionId);

        //UPDATE DATA
        bool UpdateCompetencyAssessmentName(int competencyAssessmentId, int adminId, string competencyAssessmentName);

        bool UpdateCompetencyRoleProfileLinks(int competencyAssessmentId, int adminId, int? professionalGroupId, int? subGroupId, int? roleId);
        bool UpdateCompetencyAssessmentBranding(
            int competencyAssessmentId,
            int adminId,
            int brandId,
            int categoryId
        );
        bool UpdateCompetencyAssessmentVocabulary(int competencyAssessmentId, int adminId, string vocabulary);
        bool UpdateCompetencyAssessmentDescription(int competencyAssessmentId, int adminId, string competencyAssessmentDescription);
        bool UpdateIntroductoryTextTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateBrandingTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateVocabularyTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateRoleProfileLinksTaskStatus(int assessmentId, bool taskStatus);
        bool UpdateFrameworkLinksTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool RemoveSelfAssessmentFramework(int assessmentId, int frameworkId, int adminId);
        bool UpdateSelectCompetenciesTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateOptionalCompetenciesTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateRoleRequirementsTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateWorkingGroupTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus);
        bool UpdateCompetencyAssessmentOptions(
           bool includeLearnerDeclarationPrompt,
           bool includesSignposting,
           bool linearNavigation,
           bool useDescriptionExpanders,
           string? questionLabelText,
           string? reviewerCommentsLabelText,
           int competencyAssessmentId, int adminId);
        bool UpdateCompetencyAssessmentOptionsTaskStatus(int assessmentId, bool taskStatus);
        void MoveCompetencyInSelfAssessment(int competencyAssessmentId,
            int competencyId,
            string direction
        );
        void MoveCompetencyGroupInSelfAssessment(int competencyAssessmentId,
            int groupId,
            string direction
        );
        bool UpdateCompetencyAssessmentFeaturesTaskStatus(int id, bool descriptionStatus, bool providerandCategoryStatus, bool vocabularyStatus,
           bool workingGroupStatus, bool AllframeworkCompetenciesStatus);
        bool UpdateCompetencyAssessmentRoleRequirementsTaskStatus(int assessmentId, bool taskStatus);
        void UpdateSelfAssessmentFromFramework(int selfAssessmentId, int? frameworkId);
        bool UpdateOptionalCompetenciesInAssessment(int selfAssessmentId, int[] groupIds, int[] selectedStructureIds);
        void UpdateMinimumOptionalCompetencies(int selfAssessmentId, int minimumOptionalCompetecies);
        void UpdateManageOptionalCompetenciesPrompt(int selfAssessmentId, string manageOptionalCompetenciesPrompt);
        bool UpdatePrimaryFrameworkCompetencies(int assessmentId, int frameworkId);
        void UpdateRoleRequirementsFlags(int assessmentId, bool enforceRoleRequirementsForSignOff, bool includeRequirementsFilters);
        int GetCountOfAsssessmentQuestionInCompetencyAssessment(int competencyAssessmentId, int assessmentQuestionId);
        bool UpdateSupervisorRolesTaskStatus(int competencyAssessmentId, bool taskCompleteChecked);
        bool UpdateSelfAssessments(int competencyAssessmentId,
                     int? supervised,
                     int? signoff,
                     int? confirm,
                    int? supervisorDeclarationValue,
                    string? supervisorCustomText,
                    int? leanerDeclarationValue,
                    string? leanerCustomText
                   );
        //INSERT DATA
        int InsertCompetencyAssessment(int adminId, int centreId, string competencyAssessmentName);
        bool InsertSelfAssessmentFramework(int adminId, int selfAssessmentId, int frameworkId);
        bool InsertCompetenciesIntoAssessmentFromFramework(int[] selectedCompetencyIds, int frameworkId, int competencyAssessmentId);
        bool InsertSelfAssessmentGroupedCompetencies(int selfAssessmentId, int? frameworkId);
        bool InsertSelfAssessmentUngroupedCompetencies(int selfAssessmentId, int? frameworkId);
        int InsertAssessmentQuestionRoleRequirementForSelfAssessment(int assessmentId, int assessmentQuestionId, int levelValue, int? levelRAG);
        int InsertCompetencyAssessmentQuestionRoleRequirement(int assessmentId, int competencyId, int assessmentQuestionId, int levelValue, int? levelRAG);
        void InsertIntoSelfAssessmentCollaboratorsFromFrameworkCollaborators(int selfAssessmentId, int? frameworkId);

        //DELETE DATA
        bool RemoveFrameworkCompetenciesFromAssessment(int competencyAssessmentId, int frameworkId);
        bool RemoveCompetencyFromAssessment(int competencyAssessmentId, int competencyId);
        bool RemoveCompetencyGroupFromAssessment(int competencyAssessmentId, int competencyGroupId);
        IEnumerable<CompetencyAssessmentCollaboratorDetail> GetCollaboratorsForCompetencyAssessmentId(int competencyAssessmentId);
        int AddCollaboratorToCompetencyAssessment(int competencyAssessmentId, string? userEmail, bool canModify, int? centreID);
        void RemoveCollaboratorFromCompetencyAssessment(int competencyAssessmentId, int id);
        CompetencyAssessmentCollaboratorNotification? GetCollaboratorNotification(int id, int invitedByAdminId);
        bool HasCompetencyWithSignpostedLearning(int competencyAssessmentId);
        bool DeleteCompetencyAssessmentQuestionRoleRequirement(int assessmentId, int? competencyId, int assessmentQuestionId, int levelValue);
    }

    public class CompetencyAssessmentDataService : ICompetencyAssessmentDataService
    {
        private const string SelfAssessmentBaseFields = @"sa.ID, sa.Name AS CompetencyAssessmentName, sa.Description, sa.BrandID,
                sa.ParentSelfAssessmentID,
                sa.[National], sa.[Public], sa.CreatedByAdminID AS OwnerAdminID,
                sa.NRPProfessionalGroupID,
                sa.NRPSubGroupID,
                sa.NRPRoleID,
                sa.PublishStatusID, sa.Vocabulary, CASE WHEN sa.CreatedByAdminID = @adminId THEN 3 WHEN sac.CanModify = 1 THEN 2 WHEN sac.CanModify = 0 THEN 1 ELSE 0 END AS UserRole,
                sa.EnforceRoleRequirementsForSignOff, sa.IncludeRequirementsFilters,
                sa.MinimumOptionalCompetencies,
                sa.ManageOptionalCompetenciesPrompt,
                sa.IncludeLearnerDeclarationPrompt, sa.IncludesSignposting, sa.LinearNavigation, sa.UseDescriptionExpanders, sa.QuestionLabel, sa.ReviewerCommentsLabel,
                sa.SupervisorSelfAssessmentReview, sa.SupervisorResultsReview ";

        private const string SelfAssessmentFields =
            @", sa.CategoryID, sa.CreatedDate,
                 (SELECT BrandName
                 FROM    Brands
                 WHERE (BrandID = sa.BrandID)) AS Brand,
                 (SELECT CategoryName
                 FROM    CourseCategories
                 WHERE (CourseCategoryID = sa.CategoryID)) AS Category,
                 (SELECT [Name]
                 FROM    SelfAssessments AS sa2
                 WHERE (ID = sa.ParentSelfAssessmentID)) AS ParentSelfAssessment,
                 (SELECT Forename + ' ' + Surname + (CASE WHEN Active = 1 THEN '' ELSE ' (Inactive)' END) AS Expr1
                 FROM  AdminUsers
                 WHERE (AdminID = sa.CreatedByAdminID)) AS Owner,
                 sa.Archived,
                 sa.LastEdit,
                STUFF((
                    SELECT 
                        ', ' + f.FrameworkName
                    FROM 
                        SelfAssessmentFrameworks saf2
                        INNER JOIN Frameworks f ON f.ID = saf2.FrameworkId
                    WHERE 
                        saf2.SelfAssessmentId = sa.ID AND saf2.RemovedDate IS NULL
                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''
                ) AS LinkedFrameworks,
                 (SELECT ProfessionalGroup
                 FROM    NRPProfessionalGroups
                 WHERE (ID = sa.NRPProfessionalGroupID)) AS NRPProfessionalGroup,
                 (SELECT SubGroup
                 FROM    NRPSubGroups
                 WHERE (ID = sa.NRPSubGroupID)) AS NRPSubGroup,
                 (SELECT RoleProfile
                 FROM    NRPRoles
                 WHERE (ID = sa.NRPRoleID)) AS NRPRole, sar.ID AS SelfAssessmentReviewID";

        private const string SelfAssessmentBaseTables =
            @"SelfAssessments AS sa LEFT OUTER JOIN
             SelfAssessmentCollaborators AS sac ON sac.SelfAssessmentID = sa.ID AND sac.AdminID = @adminId";

        private const string SelfAssessmentTables =
            @" LEFT OUTER JOIN
             SelfAssessmentReviews AS sar ON sac.ID = sar.SelfAssessmentCollaboratorID AND sar.Archived IS NULL AND sar.ReviewComplete IS NULL";

        private readonly IDbConnection connection;
        private readonly ILogger<CompetencyAssessmentDataService> logger;

        public CompetencyAssessmentDataService(IDbConnection connection, ILogger<CompetencyAssessmentDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<CompetencyAssessment> GetAllCompetencyAssessments(int adminId)
        {
            return connection.Query<CompetencyAssessment>(
                $@"SELECT {SelfAssessmentBaseFields} {SelfAssessmentFields}
                      FROM {SelfAssessmentBaseTables} {SelfAssessmentTables}",
                new { adminId }
            );
        }

        public IEnumerable<CompetencyAssessment> GetCompetencyAssessmentsForAdminId(int adminId)
        {
            return connection.Query<CompetencyAssessment>(
                $@"SELECT {SelfAssessmentBaseFields} {SelfAssessmentFields}
                      FROM {SelfAssessmentBaseTables} {SelfAssessmentTables}
                      WHERE (sa.CreatedByAdminID = @adminId) OR
             (@adminId IN
                 (SELECT AdminID
                 FROM    SelfAssessmentCollaborators
                 WHERE (SelfAssessmentID = sa.ID)))",
                new { adminId }
            );
        }

        public CompetencyAssessmentBase? GetCompetencyAssessmentBaseById(int competencyAssessmentId, int adminId)
        {
            return connection.Query<CompetencyAssessmentBase>(
                $@"SELECT {SelfAssessmentBaseFields}
                      FROM {SelfAssessmentBaseTables}
                      WHERE (sa.ID = @competencyAssessmentId)",
                new { competencyAssessmentId, adminId }
            ).FirstOrDefault();
        }

        public int InsertCompetencyAssessment(int adminId, int centreId, string competencyAssessmentName)
        {
            if ((competencyAssessmentName.Length == 0) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not inserting competency assessmente as it failed server side validation. AdminId: {adminId}, competencyAssessmentName: {competencyAssessmentName}"
                );
                return -1;
            }
            var result = connection.ExecuteScalar(
                    @"SELECT COUNT(*) FROM SelfAssessments WHERE [Name] = @competencyAssessmentName",
                    new { competencyAssessmentName }
             );
            int existingSelfAssessments = Convert.ToInt32(result);
            if (existingSelfAssessments > 0)
            {
                return -1;
            }
            var assessmentId = connection.QuerySingle<int>(
               @"INSERT INTO SelfAssessments ([Name], CreatedByCentreID, CreatedByAdminID)
                    OUTPUT INSERTED.Id
                    VALUES (@competencyAssessmentName, @centreId, @adminId)"
            ,
               new { competencyAssessmentName, centreId, adminId }
           );
            return assessmentId;
        }
        public bool UpdateCompetencyAssessmentName(int competencyAssessmentId, int adminId, string competencyAssessmentName)
        {
            if ((competencyAssessmentName.Length == 0) | (adminId < 1) | (competencyAssessmentId < 1))
            {
                logger.LogWarning(
                    $"Not updating role profile name as it failed server side validation. AdminId: {adminId}, competencyAssessmentName: {competencyAssessmentName}, competencyAssessmentId: {competencyAssessmentId}"
                );
                return false;
            }
            var result = connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM SelfAssessments WHERE [Name] = @competencyAssessmentName AND ID <> @competencyAssessmentId",
                new { competencyAssessmentName, competencyAssessmentId }
            );
            int existingSelfAssessments = Convert.ToInt32(result);
            if (existingSelfAssessments > 0)
            {
                return false;
            }

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessments SET [Name] = @competencyAssessmentName, UpdatedByAdminID = @adminId
                    WHERE ID = @competencyAssessmentId",
                new { competencyAssessmentName, adminId, competencyAssessmentId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating role profile name as db update failed. " +
                    $"SelfAssessmentName: {competencyAssessmentName}, admin id: {adminId}, competencyAssessmentId: {competencyAssessmentId}"
                );
                return false;
            }

            return true;
        }

        public IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups()
        {
            return connection.Query<NRPProfessionalGroups>(
                @"SELECT ID, ProfessionalGroup, Active
                    FROM   NRPProfessionalGroups
                    WHERE (Active = 1)
                    ORDER BY ProfessionalGroup"
            );
        }

        public CompetencyAssessmentBase? GetCompetencyAssessmentBaseByName(string competencyAssessmentName, int adminId)
        {
            return connection.Query<CompetencyAssessmentBase>(
                $@"SELECT {SelfAssessmentBaseFields}
                      FROM {SelfAssessmentBaseTables}
                      WHERE (sa.Name = @competencyAssessmentName)",
                new { competencyAssessmentName, adminId }
            ).FirstOrDefault();
        }

        public bool UpdateCompetencyRoleProfileLinks(int competencyAssessmentId, int adminId, int? professionalGroupId, int? subGroupId, int? roleId)
        {
            var result = connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM SelfAssessments WHERE ID = @competencyAssessmentId AND NRPProfessionalGroupID = @professionalGroupId AND NRPSubGroupID = @subGroupId AND NRPRoleID = @roleId",
                new { competencyAssessmentId, professionalGroupId, subGroupId, roleId }
            );
            int sameCount = Convert.ToInt32(result);
            if (sameCount > 0)
            {
                //same so don't update:
                return false;
            }

            //needs updating:
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessments SET NRPProfessionalGroupID = @professionalGroupId, NRPSubGroupID = @subGroupId, NRPRoleID = @roleId, UpdatedByAdminID = @adminId
                    WHERE ID = @competencyAssessmentId",
                new { adminId, competencyAssessmentId, professionalGroupId, subGroupId, roleId }
            );
            if (numberOfAffectedRows > 0)
            {
                return true;
            }

            return false;
        }
        public bool UpdateCompetencyAssessmentBranding(
            int competencyAssessmentId,
            int adminId,
            int brandId,
            int categoryId
        )
        {
            if ((competencyAssessmentId < 1) | (brandId < 1) | (categoryId < 1) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not updating competency assessment as it failed server side validation. competencyAssessmentId: {competencyAssessmentId}, brandId: {brandId}, categoryId: {categoryId},  AdminId: {adminId}"
                );
                return false;
            }

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessments SET BrandID = @brandId, CategoryID = @categoryId, UpdatedByAdminID = @adminId
                    WHERE ID = @competencyAssessmentId",
                new { brandId, categoryId, adminId, competencyAssessmentId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating competency assessment branding as db update failed. " +
                    $"frameworkId: {competencyAssessmentId}, brandId: {brandId}, categoryId: {categoryId},  AdminId: {adminId}"
                );
                return false;
            }

            return true;
        }

        public bool UpdateCompetencyAssessmentDescription(int competencyAssessmentId, int adminId, string competencyAssessmentDescription)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessments SET Description = @competencyAssessmentDescription, UpdatedByAdminID = @adminId
                    WHERE ID = @competencyAssessmentId",
                new { adminId, competencyAssessmentId, competencyAssessmentDescription }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating competency assessment Description as db update failed. " +
                    $"frameworkId: {competencyAssessmentId}, competencyAssessmentDescription: {competencyAssessmentDescription}, AdminId: {adminId}"
                );
                return false;
            }
            return true;
        }

        public bool UpdateCompetencyAssessmentVocabulary(int competencyAssessmentId, int adminId, string vocabulary)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessments SET Vocabulary = @vocabulary, UpdatedByAdminID = @adminId
                    WHERE ID = @competencyAssessmentId",
                new { adminId, competencyAssessmentId, vocabulary }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating competency assessment vocabulary as db update failed. " +
                    $"frameworkId: {competencyAssessmentId}, vocabulary: {vocabulary}, AdminId: {adminId}"
                );
                return false;
            }
            return true;
        }

        public bool InsertSelfAssessmentFramework(int adminId, int selfAssessmentId, int frameworkId)
        {
            bool isPrimary = Convert.ToInt32(connection.ExecuteScalar(
                @"SELECT Count(1) FROM SelfAssessmentFrameworks
                    WHERE SelfAssessmentId = @selfAssessmentId AND IsPrimary = 1", new { selfAssessmentId })) == 0;

            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO SelfAssessmentFrameworks (SelfAssessmentId, FrameworkId, CreatedByAdminId, IsPrimary)
                    SELECT @selfAssessmentId, @frameworkId, @adminId, @isPrimary
                    WHERE NOT EXISTS (SELECT 1 FROM SelfAssessmentFrameworks WHERE SelfAssessmentId = @selfAssessmentId AND FrameworkId = @frameworkId)"
            ,
                new { adminId, selfAssessmentId, frameworkId, isPrimary }
            );
            if (numberOfAffectedRows < 1)
            {
                numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessmentFrameworks
                    SET RemovedDate = NULL, RemovedByAdminId = NULL, AmendedByAdminId = @adminId, IsPrimary = 0
                    WHERE SelfAssessmentId = @selfAssessmentId AND FrameworkId = @frameworkId"
            ,
                new { adminId, selfAssessmentId, frameworkId }
            );
            }
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                "Not inserting SelfAssessmentFrameworks record as db insert failed. " +
                    $"selfAssessmentId: {selfAssessmentId}, frameworkId: {frameworkId}, AdminId: {adminId}"
                );
                return false;
            }

            return true;
        }

        public CompetencyAssessmentTaskStatus GetOrInsertAndReturnAssessmentTaskStatus(int assessmentId, bool frameworkBased)
        {
            bool? frameworkItemBool = frameworkBased ? false : null;
            connection.Execute(
           @"INSERT INTO SelfAssessmentTaskStatus (SelfAssessmentId, IntroductoryTextTaskStatus, BrandingTaskStatus, VocabularyTaskStatus, FrameworkLinksTaskStatus)
                    SELECT @assessmentId, @frameworkItemBool, @frameworkItemBool, @frameworkItemBool, @frameworkItemBool
                    WHERE NOT EXISTS (SELECT 1 FROM SelfAssessmentTaskStatus WHERE SelfAssessmentId = @assessmentId)", new { assessmentId, frameworkItemBool });
            return connection.Query<CompetencyAssessmentTaskStatus>(
                $@"SELECT *
                      FROM SelfAssessmentTaskStatus
                      WHERE (SelfAssessmentId = @assessmentId)",
                new { assessmentId }
            ).Single();

        }
        public bool UpdateIntroductoryTextTaskStatus(int assessmentId, bool taskStatus)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessmentTaskStatus SET IntroductoryTextTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId",
                new { assessmentId, taskStatus }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating IntroductoryTextTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }

        public CompetencyAssessment? GetCompetencyAssessmentById(int competencyAssessmentId, int adminId)
        {
            return connection.Query<CompetencyAssessment>(
                $@"SELECT {SelfAssessmentBaseFields} {SelfAssessmentFields}
                      FROM {SelfAssessmentBaseTables} {SelfAssessmentTables}
                      WHERE (sa.ID = @competencyAssessmentId)",
                new { competencyAssessmentId, adminId }
            ).FirstOrDefault();
        }

        public bool UpdateBrandingTaskStatus(int assessmentId, bool taskStatus)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessmentTaskStatus SET BrandingTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId",
                new { assessmentId, taskStatus }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating BrandingTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }

        public bool UpdateVocabularyTaskStatus(int assessmentId, bool taskStatus)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessmentTaskStatus SET VocabularyTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId",
                new { assessmentId, taskStatus }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating VocabularyTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }

        public IEnumerable<NRPSubGroups> GetNRPSubGroups(int? nRPProfessionalGroupID)
        {
            return connection.Query<NRPSubGroups>(
                @"SELECT ID, SubGroup, Active
                    FROM   NRPSubGroups
                    WHERE (Active = 1) AND (NRPProfessionalGroupID = @nRPProfessionalGroupID)
                    ORDER BY SubGroup", new { nRPProfessionalGroupID }
            );
        }

        public IEnumerable<NRPRoles> GetNRPRoles(int? nRPSubGroupID)
        {
            return connection.Query<NRPRoles>(
                @"SELECT ID, RoleProfile AS ProfileName, Active
                    FROM   NRPRoles
                    WHERE (Active = 1) AND (NRPSubGroupID = @nRPSubGroupID)
                    ORDER BY RoleProfile", new { nRPSubGroupID }
            );
        }

        public bool UpdateRoleProfileLinksTaskStatus(int assessmentId, bool taskStatus)
        {
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE SelfAssessmentTaskStatus SET NationalRoleProfileTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId",
               new { assessmentId, taskStatus }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating NationalRoleProfileTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }

        public int[] GetLinkedFrameworkIds(int assessmentId)
        {
            return [.. connection.Query<int>(
              @"SELECT FrameworkId
                    FROM   SelfAssessmentFrameworks
                    WHERE (SelfAssessmentId = @assessmentId) AND (RemovedDate IS NULL) AND (IsPrimary = 0)
                    ORDER BY ID",
              new { assessmentId }
          )];
        }

        public bool RemoveSelfAssessmentFramework(int assessmentId, int frameworkId, int adminId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessmentFrameworks SET RemovedDate = @removedDate, RemovedByAdminId = @adminId
                    WHERE SelfAssessmentId = @assessmentId AND FrameworkId = @frameworkId",
                new { removedDate = DateTime.Now, assessmentId, frameworkId, adminId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating SelfAssessmentFrameworks as db update failed. " +
                    $"assessmentId: {assessmentId}, frameworkId: {frameworkId}, adminId: {adminId}"
                );
                return false;
            }
            return true;
        }

        public int? GetPrimaryLinkedFrameworkId(int assessmentId)
        {
            return connection.QuerySingleOrDefault<int?>(
              @"SELECT TOP(1) FrameworkId
                    FROM   SelfAssessmentFrameworks
                    WHERE (SelfAssessmentId = @assessmentId) AND (RemovedDate IS NULL) AND (IsPrimary = 1)
                    ORDER BY ID DESC",
              new { assessmentId }
          );
        }

        public bool UpdateFrameworkLinksTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE SelfAssessmentTaskStatus SET FrameworkLinksTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId  AND (@previousStatus IS NULL OR FrameworkLinksTaskStatus = @previousStatus)",
               new { assessmentId, taskStatus, previousStatus }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating FrameworkLinksTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }

        public int GetCompetencyCountByFrameworkId(int assessmentId, int frameworkId)
        {
            return connection.ExecuteScalar<int>(
              @"SELECT COUNT(sas.CompetencyID) AS Competencies
                   FROM   SelfAssessmentStructure AS sas INNER JOIN
                             FrameworkCompetencies AS fc ON sas.CompetencyID = fc.CompetencyID INNER JOIN
                             SelfAssessmentFrameworks AS saf ON fc.FrameworkID = saf.FrameworkId AND sas.SelfAssessmentID = saf.SelfAssessmentId
                    WHERE (saf.SelfAssessmentId = @assessmentId) AND (saf.FrameworkId = @frameworkId)",
              new { assessmentId, frameworkId }
          );
        }

        public bool RemoveFrameworkCompetenciesFromAssessment(int competencyAssessmentId, int frameworkId)
        {
            var numberOfAffectedRows = connection.Execute(
               @"DELETE FROM SelfAssessmentStructure
                    FROM   SelfAssessmentStructure INNER JOIN
                                 FrameworkCompetencies AS fc ON SelfAssessmentStructure.CompetencyID = fc.CompetencyID INNER JOIN
                                 SelfAssessmentFrameworks AS saf ON fc.FrameworkID = saf.FrameworkId AND SelfAssessmentStructure.SelfAssessmentID = saf.SelfAssessmentId
                    WHERE (saf.SelfAssessmentId = @competencyAssessmentId) AND (saf.FrameworkId = @frameworkId)",
               new { competencyAssessmentId, frameworkId }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not removing competencies linked to source framework as db update failed. " +
                    $"assessmentId: {competencyAssessmentId}, taskStatus: {frameworkId}"
                );
                return false;
            }
            return true;
        }

        public bool UpdateSelectCompetenciesTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE SelfAssessmentTaskStatus SET SelectCompetenciesTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId AND (@previousStatus IS NULL OR SelectCompetenciesTaskStatus = @previousStatus)",
               new { assessmentId, taskStatus, previousStatus }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating SelectCompetenciesTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }
        public bool UpdateOptionalCompetenciesTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE SelfAssessmentTaskStatus SET OptionalCompetenciesTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId AND (@previousStatus IS NULL OR OptionalCompetenciesTaskStatus = @previousStatus)",
               new { assessmentId, taskStatus, previousStatus }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating OptionalCompetenciesTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }
        public bool UpdateRoleRequirementsTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE SelfAssessmentTaskStatus SET RoleRequirementsTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId AND (@previousStatus IS NULL OR RoleRequirementsTaskStatus = @previousStatus)",
               new { assessmentId, taskStatus, previousStatus }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating RoleRequirementsTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }
        public bool UpdateWorkingGroupTaskStatus(int assessmentId, bool taskStatus, bool? previousStatus)
        {
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE SelfAssessmentTaskStatus SET WorkingGroupTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId AND (@previousStatus IS NULL OR WorkingGroupTaskStatus = @previousStatus)",
               new { assessmentId, taskStatus, previousStatus }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating WorkingGroupTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }

        public IEnumerable<Competency> GetCompetenciesForCompetencyAssessment(int competencyAssessmentId)
        {
            return connection.Query<Competency>(
               @"SELECT sas.ID AS StructureId, sas.CompetencyID, f.ID AS FrameworkId, f.FrameworkName, cg.ID AS GroupId, cg.Name AS GroupName, c.Name AS CompetencyName, c.Description AS CompetencyDescription, sas.Optional, sas.GroupOptionalCompetencies
                    FROM   SelfAssessmentStructure AS sas INNER JOIN
                                Competencies AS c ON sas.CompetencyID = c.ID LEFT JOIN
                                CompetencyGroups AS cg ON sas.CompetencyGroupID = cg.ID INNER JOIN
                                FrameworkCompetencies ON c.ID = FrameworkCompetencies.CompetencyID INNER JOIN
                                Frameworks AS f ON FrameworkCompetencies.FrameworkID = f.ID INNER JOIN
                                SelfAssessmentFrameworks ON f.ID = SelfAssessmentFrameworks.FrameworkId AND sas.SelfAssessmentID = SelfAssessmentFrameworks.SelfAssessmentId
                    WHERE (sas.SelfAssessmentID = @competencyAssessmentId)
                    ORDER BY sas.Ordering", new { competencyAssessmentId }
           );
        }

        public IEnumerable<LinkedFramework> GetLinkedFrameworksForCompetencyAssessment(int competencyAssessmentId)
        {
            return connection.Query<LinkedFramework>(
                @"SELECT f.ID,
            FrameworkName,
            f.OwnerAdminID,
            f.BrandID,
            f.CategoryID,
            f.TopicID,
            f.CreatedDate,
            f.PublishStatusID,
            f.UpdatedByAdminID
                    FROM   SelfAssessmentFrameworks saf INNER JOIN
                                Frameworks AS f ON saf.FrameworkId = f.ID
                    WHERE (saf.SelfAssessmentId = @competencyAssessmentId) AND (saf.RemovedDate IS NULL)
                    ORDER BY f.FrameworkName", new { competencyAssessmentId }
            );
        }

        public int[] GetLinkedFrameworkCompetencyIds(int competencyAssessmentId, int frameworkId)
        {
            return [.. connection.Query<int>(
              @"SELECT sas.CompetencyID
                    FROM   SelfAssessmentStructure AS sas INNER JOIN
                         FrameworkCompetencies AS fc ON sas.CompetencyID = fc.CompetencyID
                    WHERE (sas.SelfAssessmentID = @competencyAssessmentId) AND (fc.FrameworkID = @frameworkId)
                    ORDER BY fc.Ordering",
              new { competencyAssessmentId,  frameworkId}
          )];
        }

        public bool InsertCompetenciesIntoAssessmentFromFramework(int[] selectedCompetencyIds, int frameworkId, int competencyAssessmentId)
        {

            var currentMaxOrdering = connection.ExecuteScalar<int>(
                @"SELECT ISNULL(MAX(Ordering), 0) FROM SelfAssessmentStructure WHERE SelfAssessmentID = @competencyAssessmentId",
                new { competencyAssessmentId }
            );
            var numberOfAffectedRows = connection.Execute(
               @"INSERT INTO SelfAssessmentStructure (SelfAssessmentID, CompetencyID, Ordering, CompetencyGroupID)
                    SELECT
                        @competencyAssessmentId,
                        FC.CompetencyID,
                        ROW_NUMBER() OVER (ORDER BY FCG.Ordering, FC.Ordering) + @currentMaxOrdering,
                        FCG.CompetencyGroupID
                    FROM FrameworkCompetencies AS FC
                    LEFT JOIN FrameworkCompetencyGroups AS FCG ON FC.FrameworkCompetencyGroupID = FCG.ID
                    WHERE FC.FrameworkID = @frameworkId
                    AND FC.CompetencyID IN @selectedCompetencyIds AND FC.CompetencyID NOT IN (SELECT CompetencyID FROM SelfAssessmentStructure WHERE SelfAssessmentID = @competencyAssessmentId)",
               new { selectedCompetencyIds, frameworkId, competencyAssessmentId, currentMaxOrdering }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not inserting competencies into assessment as db update failed. " +
                    $"assessmentId: {competencyAssessmentId}, frameworkId: {frameworkId}, selectedCompetencyIds: {selectedCompetencyIds}"
                );
                return false;
            }
            return true;
        }
        public bool RemoveCompetencyFromAssessment(int competencyAssessmentId, int competencyId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM SelfAssessmentStructure
                    WHERE SelfAssessmentID = @competencyAssessmentId AND CompetencyID = @competencyId",
                new { competencyAssessmentId, competencyId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not removing competency from assessment as db update failed. " +
                    $"assessmentId: {competencyAssessmentId}, competencyId: {competencyId}"
                );
                return false;
            }
            return true;
        }

        public bool RemoveCompetencyGroupFromAssessment(int competencyAssessmentId, int competencyGroupId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM SelfAssessmentStructure
                    WHERE SelfAssessmentID = @competencyAssessmentId AND CompetencyGroupId = @competencyGroupId",
                new { competencyAssessmentId, competencyGroupId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not removing competency from assessment as db update failed. " +
                    $"assessmentId: {competencyAssessmentId}, competencyGroupId: {competencyGroupId}"
                );
                return false;
            }
            return true;
        }

        public void MoveCompetencyInSelfAssessment(int competencyAssessmentId, int competencyId, string direction)
        {
            connection.Execute(
                "usp_MoveCompetencyInSelfAssessment",
                    new { SelfAssessmentID = competencyAssessmentId, CompetencyID = competencyId, Direction = direction },
                    commandType: CommandType.StoredProcedure
                );

        }

        public void MoveCompetencyGroupInSelfAssessment(int competencyAssessmentId, int groupId, string direction)
        {
            connection.Execute(
                "usp_MoveCompetencyGroupInSelfAssessment",
                    new { SelfAssessmentID = competencyAssessmentId, GroupID = groupId, Direction = direction },
                    commandType: CommandType.StoredProcedure
                );
        }

        public bool UpdateCompetencyAssessmentFeaturesTaskStatus(int id, bool descriptionStatus, bool providerandCategoryStatus, bool vocabularyStatus,
            bool workingGroupStatus, bool AllframeworkCompetenciesStatus)
        {
            var numberOfAffectedRows = connection.Execute(
               @"IF EXISTS (SELECT 1 FROM SelfAssessmentTaskStatus WHERE SelfAssessmentId = @id)
               BEGIN
               UPDATE SelfAssessmentTaskStatus
                SET IntroductoryTextTaskStatus =
                 CASE WHEN @descriptionStatus = 1 AND IntroductoryTextTaskStatus <> 1
                 THEN 0 ELSE IntroductoryTextTaskStatus END,
                 BrandingTaskStatus =
                  CASE WHEN @providerandCategoryStatus = 1 AND BrandingTaskStatus <> 1
                    THEN 0 ELSE BrandingTaskStatus END,
                    VocabularyTaskStatus =
                 CASE WHEN @vocabularyStatus = 1 AND VocabularyTaskStatus <> 1
                  THEN 0 ELSE VocabularyTaskStatus END,
                WorkingGroupTaskStatus =
                 CASE WHEN @workingGroupStatus = 1 AND WorkingGroupTaskStatus <> 1
                 THEN 0 ELSE WorkingGroupTaskStatus END,
                 FrameworkLinksTaskStatus =
                  CASE WHEN @AllframeworkCompetenciesStatus = 1 AND FrameworkLinksTaskStatus <> 1
                  THEN 0 ELSE FrameworkLinksTaskStatus END,
                 SelectCompetenciesTaskStatus =
                  CASE WHEN @AllframeworkCompetenciesStatus = 1 AND SelectCompetenciesTaskStatus <> 1
                  THEN 0 ELSE SelectCompetenciesTaskStatus END
                 WHERE SelfAssessmentId = @id;
                    END
                     ELSE
                    BEGIN
                 INSERT INTO SelfAssessmentTaskStatus
                 (SelfAssessmentId, IntroductoryTextTaskStatus, BrandingTaskStatus, VocabularyTaskStatus, WorkingGroupTaskStatus,
                FrameworkLinksTaskStatus,SelectCompetenciesTaskStatus)
                 VALUES
                 (
                       @id,
                    CASE WHEN @descriptionStatus = 1 THEN 0 ELSE NULL END,
                    CASE WHEN @providerandCategoryStatus = 1 THEN 0 ELSE NULL END,
                    CASE WHEN @vocabularyStatus = 1 THEN 0 ELSE NULL END,
                    CASE WHEN @workingGroupStatus = 1 THEN 0 ELSE NULL END,
                    CASE WHEN @AllframeworkCompetenciesStatus = 1 THEN 0 ELSE NULL END,
                    CASE WHEN @AllframeworkCompetenciesStatus = 1 THEN 0 ELSE NULL END
                        );
                        END",
               new { id, descriptionStatus, providerandCategoryStatus, vocabularyStatus, workingGroupStatus, AllframeworkCompetenciesStatus }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating SelfAssessmentTaskStatus as db update failed. " +
                    $"SelfAssessmentId: {id}, IntroductoryTextTaskStatus: {descriptionStatus}, BrandingTaskStatus: {providerandCategoryStatus}, " +
                    $"VocabularyTaskStatus: {vocabularyStatus}, WorkingGroupTaskStatus: {workingGroupStatus}, FrameworkLinksTaskStatus: {AllframeworkCompetenciesStatus}"
                );
                return false;
            }
            return true;
        }

        public CompetencyAssessmentFeatures? GetCompetencyAssessmentFeaturesTaskStatus(int competencyAssessmentId)
        {
            return connection.QueryFirstOrDefault<CompetencyAssessmentFeatures>(
               @"SELECT s.ID, s.Name AS CompetencyAssessmentName, sts.IntroductoryTextTaskStatus AS DescriptionStatus, sts.BrandingTaskStatus AS ProviderandCategoryStatus,
	           sts.VocabularyTaskStatus AS VocabularyStatus, sts.WorkingGroupTaskStatus AS WorkingGroupStatus, sts.FrameworkLinksTaskStatus AS AllframeworkCompetenciesStatus
	           FROM  SelfAssessments s INNER JOIN 
	           SelfAssessmentTaskStatus sts ON s.ID = sts.SelfAssessmentId 
	          WHERE s.ID = @competencyAssessmentId",
               new { competencyAssessmentId }
           );

        }

        public void UpdateSelfAssessmentFromFramework(int selfAssessmentId, int? frameworkId)
        {

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE s
                    SET
                    [Description] = CASE 
                    WHEN sts.IntroductoryTextTaskStatus IS NULL THEN NULL
                    ELSE COALESCE(F.[Description], 'No description provided')
                    END,
                    BrandID = CASE 
                    WHEN sts.BrandingTaskStatus IS NULL THEN s.BrandID
                    ELSE F.BrandID
                    END,
                    CategoryID = CASE 
                    WHEN sts.BrandingTaskStatus IS NULL THEN s.CategoryID
                    ELSE F.CategoryID
                    END,
                    Vocabulary = CASE 
                    WHEN sts.VocabularyTaskStatus IS NULL THEN NULL
                    ELSE F.FrameworkConfig
                    END
                    FROM SelfAssessments s
                    INNER JOIN Frameworks F ON F.ID = @frameworkId
                    INNER JOIN AdminUsers AU ON F.OwnerAdminID = AU.AdminID
                    INNER JOIN SelfAssessmentTaskStatus sts ON s.ID = sts.SelfAssessmentId
                    WHERE s.ID = @selfAssessmentId;",
                new { selfAssessmentId, frameworkId }
            );
        }
        public bool InsertSelfAssessmentGroupedCompetencies(int selfAssessmentId, int? frameworkId)
        {

            var numberOfAffectedRows = connection.Execute(
            @"INSERT INTO SelfAssessmentStructure (SelfAssessmentID, CompetencyID, Ordering, CompetencyGroupID)
                SELECT s.ID, FC.CompetencyID, ROW_NUMBER() OVER( ORDER BY FCG.Ordering, FC.Ordering ), FCG.CompetencyGroupID
                 FROM FrameworkCompetencies AS FC 
                INNER JOIN FrameworkCompetencyGroups AS FCG ON FC.FrameworkCompetencyGroupID = FCG.ID INNER JOIN
				SelfAssessments s ON s.id = @selfAssessmentId
                WHERE FC.FrameworkID = @frameworkId"
        ,
            new { selfAssessmentId, frameworkId }
        );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                "Not inserting SelfAssessmentStructure record as db insert failed. " +
                    $"selfAssessmentId: {selfAssessmentId}, frameworkId: {frameworkId}"
                );
                return false;
            }

            return true;
        }
        public bool InsertSelfAssessmentUngroupedCompetencies(int selfAssessmentId, int? frameworkId)
        {

            var numberOfAffectedRows = connection.Execute(
            @"INSERT INTO SelfAssessmentStructure (SelfAssessmentID, CompetencyID, Ordering)
            SELECT s.ID, FC.CompetencyID,  FC.Ordering 
            FROM FrameworkCompetencies AS fc               
            INNER JOIN SelfAssessments s ON s.id = @selfAssessmentId      
            WHERE fc.FrameworkID = @frameworkId            
            AND fc.FrameworkCompetencyGroupID IS NULL "
        ,
            new { selfAssessmentId, frameworkId }
        );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                "Not inserting SelfAssessmentStructure record as db insert failed. " +
                    $"selfAssessmentId: {selfAssessmentId}, frameworkId: {frameworkId}"
                );
                return false;
            }

            return true;
        }

        public bool UpdatePrimaryFrameworkCompetencies(int assessmentId, int frameworkId)
        {
            connection.EnsureOpen();
            using (var transaction = connection.BeginTransaction())
            {
                var numberOfAffectedRows = connection.Execute(
                    @"UPDATE SelfAssessmentFrameworks 
                  SET IsPrimary = 0  
                  WHERE (SelfAssessmentId = @assessmentId) 
                    AND (RemovedDate IS NULL)",
                    new { assessmentId },
                    transaction: transaction
                );

                var numberOfAffectedRow = connection.Execute(
                    @"UPDATE SelfAssessmentFrameworks 
                  SET IsPrimary = 1  
                  WHERE (SelfAssessmentId = @assessmentId) 
                    AND (FrameworkId = @frameworkId) 
                    AND (RemovedDate IS NULL)",
                    new { assessmentId, frameworkId },
                    transaction: transaction
                );

                if ((numberOfAffectedRow < 1) || (numberOfAffectedRows < 1))
                {
                    logger.LogWarning(
                        "Not updating SelfAssessmentFrameworks as db update failed. " +
                        $"assessmentId: {assessmentId}, frameworkId: {frameworkId}"
                    );
                    transaction.Rollback();
                    return false;
                }

                transaction.Commit();
                return true;
            }
        }
        public int? GetSelfAssessmentStructure(int competencyAssessmentId)
        {
            return connection.QueryFirstOrDefault<int>(
               @"SELECT 1 from dbo.SelfAssessmentStructure where selfassessmentid  = @competencyAssessmentId",
               new { competencyAssessmentId }
               );
        }
        public IEnumerable<CompetencyAssessmentCollaboratorDetail> GetCollaboratorsForCompetencyAssessmentId(int competencyAssessmentId)
        {
            return connection.Query<CompetencyAssessmentCollaboratorDetail>(
                @"SELECT
                        0 AS ID,
                        sa.ID AS SelfAssessmentID,
                        au.AdminID AS AdminID,
                        1 AS CanModify,
                        au.Email AS UserEmail,
                        au.Active AS UserActive,
                        'Owner' AS CompetencyAssessmentRole
                    FROM SelfAssessments AS sa
                    INNER JOIN AdminUsers AS au ON sa.CreatedByAdminID = au.AdminID
                    WHERE (sa.ID = @competencyAssessmentId)
                    UNION ALL
                    SELECT
                        ID,
                        SelfAssessmentID,
                        sc.AdminID,
                        CanModify,
                        UserEmail,
                        au.Active AS UserActive,
                        CASE WHEN CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS CompetencyAssessmentRole
                    FROM SelfAssessmentCollaborators sc
                    INNER JOIN AdminUsers AS au ON sc.AdminID = au.AdminID
                        AND sc.IsDeleted = 0
                    WHERE (SelfAssessmentID = @competencyAssessmentId)",
                new { competencyAssessmentId }
            );
        }
        public int AddCollaboratorToCompetencyAssessment(int competencyAssessmentId, string? userEmail, bool canModify, int? centreID)
        {
            if (userEmail is null || userEmail.Length == 0)
            {
                logger.LogWarning(
                    $"Not adding collaborator to competency assessment as it failed server side valiidation. competencyAssessmentId: {competencyAssessmentId}, userEmail: {userEmail}, canModify:{canModify}"
                );
                return -3;
            }

            var existingId = connection.QuerySingle<int>(
                @"SELECT COALESCE
                     ((SELECT ID
                      FROM    SelfAssessmentCollaborators
                      WHERE (SelfAssessmentID = @competencyAssessmentId) AND (UserEmail = @userEmail) AND (IsDeleted=0)), 0) AS ID",
                new { competencyAssessmentId, userEmail }
            );
            if (existingId > 0)
            {
                return -2;
            }

            var adminId = (int?)connection.ExecuteScalar(
                @"SELECT AdminID FROM AdminUsers WHERE Email = @userEmail AND Active = 1 AND CentreID = @centreID",
                new { userEmail, centreID }
            );
            if (adminId is null)
            {
                return -4;
            }

            var ownerEmail = (string?)connection.ExecuteScalar(@"SELECT AU.Email FROM SelfAssessments SA
                            INNER JOIN AdminUsers AU ON AU.AdminID = SA.CreatedByAdminID
                            WHERE SA.ID = @competencyAssessmentId", new { competencyAssessmentId });
            if (ownerEmail == userEmail)
            {
                return -5;
            }

            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO SelfAssessmentCollaborators (SelfAssessmentID, AdminID, UserEmail, CanModify)
                    VALUES (@competencyAssessmentId, @adminId, @userEmail, @canModify)",
                new { competencyAssessmentId, adminId, userEmail, canModify }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not inserting framework collaborator as db insert failed. AdminId: {adminId}, userEmail: {userEmail}, competencyAssessmentId: {competencyAssessmentId}, canModify: {canModify}"
                );
                return -1;
            }

            if (adminId > 0)
            {
                connection.Execute(
                    @"UPDATE AdminUsers SET IsWorkforceManager = 1 WHERE AdminId = @adminId AND IsWorkforceManager = 0",
                    new { adminId }
                );
            }

            existingId = connection.QuerySingle<int>(
                @"SELECT COALESCE
                     ((SELECT ID
                      FROM    SelfAssessmentCollaborators
                      WHERE (SelfAssessmentID = @competencyAssessmentId) AND (UserEmail = @userEmail) AND (IsDeleted=0)), 0) AS AdminID",
                new { competencyAssessmentId, adminId, userEmail }
            );
            return existingId;
        }

        public void RemoveCollaboratorFromCompetencyAssessment(int competencyAssessmentId, int id)
        {
            var adminId = (int?)connection.ExecuteScalar(
                @"SELECT AdminID FROM SelfAssessmentCollaborators WHERE  (SelfAssessmentID = @competencyAssessmentId) AND (ID = @id)",
                new { competencyAssessmentId, id }
            );
            connection.Execute(
                @"UPDATE SelfAssessmentCollaborators SET IsDeleted=1 WHERE (SelfAssessmentID = @competencyAssessmentId) AND (ID = @id);
                    UPDATE AdminUsers SET IsWorkforceManager = 0 WHERE AdminID = @adminId AND AdminID NOT IN (SELECT DISTINCT AdminID FROM SelfAssessmentCollaborators);",
                new { competencyAssessmentId, id, adminId }
            );
        }
        public CompetencyAssessmentCollaboratorNotification? GetCollaboratorNotification(int id, int invitedByAdminId)
        {
            return connection.Query<CompetencyAssessmentCollaboratorNotification>(
                @"SELECT
                    sc.SelfAssessmentID,
                    sc.AdminID,
                    sc.CanModify,
                    sc.UserEmail,
                    au.Active AS UserActive,
                    CASE WHEN sc.CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS CompetencyAssessmentRole,
                    sa.[Name] AS CompetencyAssessmentName,
                    (SELECT Forename + ' ' + Surname + (CASE WHEN Active = 1 THEN '' ELSE ' (Inactive)' END) AS Expr1 FROM AdminUsers AS au1 WHERE (AdminID = @invitedByAdminId)) AS InvitedByName,
                    (SELECT Email FROM AdminUsers AS au2 WHERE (AdminID = @invitedByAdminId)) AS InvitedByEmail
                FROM SelfAssessmentCollaborators AS sc
                INNER JOIN SelfAssessments AS sa ON sc.SelfAssessmentID = sa.ID
                INNER JOIN AdminUsers AS au ON sc.AdminID = au.AdminID
                WHERE (sc.ID = @id) AND (sc.IsDeleted=0)",
                new { invitedByAdminId, id }
            ).FirstOrDefault();
        }

        public bool HasCompetencyWithSignpostedLearning(int competencyAssessmentId)
        {
            int hasSignpostedLearning = connection.QueryFirstOrDefault<int>(
                @"SELECT count(*) FROM SelfAssessmentStructure sas INNER JOIN 
			        CompetencyLearningResources clr ON sas.CompetencyID = clr.CompetencyID AND
			        clr.RemovedDate IS NULL
		        WHERE sas.SelfAssessmentID = @competencyAssessmentId",
                new { competencyAssessmentId });

            return hasSignpostedLearning > 0;
        }
        public bool UpdateCompetencyAssessmentOptions(
            bool includeLearnerDeclarationPrompt,
            bool includesSignposting,
            bool linearNavigation,
            bool useDescriptionExpanders,
            string? questionLabelText,
            string? reviewerCommentsLabelText,
            int competencyAssessmentId, int adminId)
        {
            if ((adminId < 1) | (competencyAssessmentId < 1))
            {
                logger.LogWarning(
                    $"Not updating role profile name as it failed server side validation. AdminId: {adminId}, competencyAssessmentId: {competencyAssessmentId}"
                );
                return false;
            }

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessments SET IncludeLearnerDeclarationPrompt = @includeLearnerDeclarationPrompt,
                    IncludesSignposting = @includesSignposting,
                    LinearNavigation = @linearNavigation,
                    UseDescriptionExpanders = @useDescriptionExpanders,
                    QuestionLabel = @questionLabelText,
                    ReviewerCommentsLabel = @reviewerCommentsLabelText,
                    UpdatedByAdminID = @adminId
                    WHERE ID = @competencyAssessmentId ",
                new
                {
                    includeLearnerDeclarationPrompt,
                    includesSignposting,
                    linearNavigation,
                    useDescriptionExpanders,
                    questionLabelText,
                    reviewerCommentsLabelText,
                    adminId,
                    competencyAssessmentId
                }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating options/labels as db update failed. " +
                    $"admin id: {adminId}, competencyAssessmentId: {competencyAssessmentId}"
                );
                return false;
            }

            return true;
        }

        public bool UpdateCompetencyAssessmentOptionsTaskStatus(int assessmentId, bool taskStatus)
        {
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE SelfAssessmentTaskStatus SET SelfAssessmentOptionsTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId",
               new { assessmentId, taskStatus }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating SelfAssessmentOptionsTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }

        public bool UpdateOptionalCompetenciesInAssessment(int selfAssessmentId, int[] groupIds, int[] selectedStructureIds)
        {
            const string sql = @"UPDATE sas
                                 SET 
                                    Optional =
                                        CASE 
                                            WHEN sas.CompetencyGroupID IN @GroupIds THEN 1
                                            WHEN sas.ID IN @SelectedIds THEN 1
                                            ELSE 0
                                        END,
                                    GroupOptionalCompetencies =
                                        CASE 
                                            WHEN sas.CompetencyGroupID IN @GroupIds THEN 1
                                            ELSE 0
                                        END
                                 FROM SelfAssessmentStructure sas
                                    WHERE sas.SelfAssessmentID = @SelfAssessmentId;";

            var safeGroupIds = (groupIds != null && groupIds.Length > 0)
                ? groupIds
                : [-1];

            var safeSelectedIds = (selectedStructureIds != null && selectedStructureIds.Length > 0)
                ? selectedStructureIds
                : [-1];


            var rows = connection.Execute(sql, new
            {
                SelfAssessmentId = selfAssessmentId,
                GroupIds = safeGroupIds,
                SelectedIds = safeSelectedIds
            });

            // Returns true if any rows were updated
            return rows > 0;
        }

        public void UpdateMinimumOptionalCompetencies(int selfAssessmentId, int minimumOptionalCompetecies)
        {
            connection.Execute(
                @"UPDATE SelfAssessments
                    SET 
                    [MinimumOptionalCompetencies] = @minimumOptionalCompetecies
                    WHERE id = @selfAssessmentId AND ISNULL(MinimumOptionalCompetencies, 0) <> @minimumOptionalCompetecies;"
            ,
                new { selfAssessmentId, minimumOptionalCompetecies }
            );
        }
        public void UpdateManageOptionalCompetenciesPrompt(int selfAssessmentId, string? manageOptionalCompetenciesPrompt)
        {
            connection.Execute(
                @"UPDATE SelfAssessments
                    SET 
                    [ManageOptionalCompetenciesPrompt] = @manageOptionalCompetenciesPrompt
                    WHERE id = @selfAssessmentId AND ISNULL(ManageOptionalCompetenciesPrompt, '') <> @manageOptionalCompetenciesPrompt;"
            ,
                new { selfAssessmentId, manageOptionalCompetenciesPrompt }
            );
        }

        public IEnumerable<CompetencyWithAssessmentQuestionRoleRequirements> GetCompetencyWithAssessmentQuestionRoleRequirements(int competencyAssessmentId, int? competencyId, int? assessmentQuestionId)
        {
            return connection.Query<CompetencyWithAssessmentQuestionRoleRequirements>(
                 @"SELECT
    sas.CompetencyGroupID,
    cg.Name AS GroupName,
    sas.CompetencyID,
    c.Name AS Competency,
    c.Description AS CompetencyDescription,
    sas.Optional,
    caq.AssessmentQuestionID,
    aq.Question,
    caq.Required,
    aqit.InputTypeName,
    aql.LevelValue       AS ResponseValue,
    aql.LevelLabel       AS Response,
    caqrr.LevelRAG
FROM SelfAssessmentStructure AS sas
INNER JOIN Competencies AS c
    ON sas.CompetencyID = c.ID
INNER JOIN CompetencyAssessmentQuestions AS caq
    ON c.ID = caq.CompetencyID
INNER JOIN AssessmentQuestions AS aq
    ON caq.AssessmentQuestionID = aq.ID
INNER JOIN AssessmentQuestionInputTypes AS aqit
    ON aq.AssessmentQuestionInputTypeID = aqit.ID
LEFT JOIN CompetencyGroups AS cg
    ON sas.CompetencyGroupID = cg.ID

INNER JOIN AssessmentQuestionLevels AS aql
    ON aql.AssessmentQuestionID = aq.ID

LEFT JOIN CompetencyAssessmentQuestionRoleRequirements AS caqrr
    ON caqrr.AssessmentQuestionID = aq.ID
    AND caqrr.SelfAssessmentID = sas.SelfAssessmentID
    AND caqrr.CompetencyID = sas.CompetencyID
   AND caqrr.LevelValue = aql.LevelValue

WHERE sas.SelfAssessmentID = @competencyAssessmentId
  AND (@competencyId IS NULL OR sas.CompetencyID = @competencyId)
  AND (@assessmentQuestionId IS NULL OR caq.AssessmentQuestionID = @assessmentQuestionId)

ORDER BY
    sas.Ordering,
    caq.Ordering,
    aql.LevelValue;",
                 new { competencyAssessmentId, competencyId, assessmentQuestionId }
             );
        }

        public bool UpdateCompetencyAssessmentRoleRequirementsTaskStatus(int assessmentId, bool taskStatus)
        {
            var numberOfAffectedRows = connection.Execute(
              @"UPDATE SelfAssessmentTaskStatus SET RoleRequirementsTaskStatus = @taskStatus
                    WHERE SelfAssessmentId = @assessmentId",
              new { assessmentId, taskStatus }
          );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating RoleRequirementsTaskStatus as db update failed. " +
                    $"assessmentId: {assessmentId}, taskStatus: {taskStatus}"
                );
                return false;
            }
            return true;
        }

        public void UpdateRoleRequirementsFlags(int assessmentId, bool enforceRoleRequirementsForSignOff, bool includeRequirementsFilters)
        {
            connection.Execute(
                 @"UPDATE SelfAssessments
                    SET 
                    [EnforceRoleRequirementsForSignOff] = @enforceRoleRequirementsForSignOff, [IncludeRequirementsFilters] = @includeRequirementsFilters
                    WHERE id = @assessmentId AND (EnforceRoleRequirementsForSignOff <> @enforceRoleRequirementsForSignOff OR IncludeRequirementsFilters <> @includeRequirementsFilters);"
             ,
                 new { assessmentId, enforceRoleRequirementsForSignOff, includeRequirementsFilters }
             );
        }

        public int GetCountOfAsssessmentQuestionInCompetencyAssessment(int competencyAssessmentId, int assessmentQuestionId)
        {
            return connection.QuerySingle<int>(
               @"SELECT COUNT(1) 
                    FROM   SelfAssessmentStructure AS sas INNER JOIN
                                Competencies AS c ON sas.CompetencyID = c.ID INNER JOIN
                                CompetencyAssessmentQuestions AS caq ON c.ID = caq.CompetencyID
                    WHERE (sas.SelfAssessmentID = @competencyAssessmentId) AND (caq.AssessmentQuestionID = @assessmentQuestionId)",
               new { competencyAssessmentId, assessmentQuestionId }
               );
        }

        public int InsertAssessmentQuestionRoleRequirementForSelfAssessment(int assessmentId, int assessmentQuestionId, int levelValue, int? levelRAG)
        {
            var numberOfAffectedRows = connection.Execute(
                             @"INSERT INTO CompetencyAssessmentQuestionRoleRequirements
                                     (SelfAssessmentID, CompetencyID, AssessmentQuestionID, LevelValue, LevelRAG)
                                        SELECT sas.SelfAssessmentID, sas.CompetencyID, caq.AssessmentQuestionID, @levelValue AS LevelValue, @levelRAG AS LevelRAG
                                        FROM   SelfAssessmentStructure AS sas INNER JOIN
                                             CompetencyAssessmentQuestions AS caq ON sas.CompetencyID = caq.CompetencyID
                                        WHERE (sas.SelfAssessmentID = @assessmentId) AND (caq.AssessmentQuestionID = @assessmentQuestionId);"
                         ,
                             new { assessmentId, assessmentQuestionId, levelValue, levelRAG }
                         );
            return numberOfAffectedRows;
        }

        public int InsertCompetencyAssessmentQuestionRoleRequirement(int assessmentId, int competencyId, int assessmentQuestionId, int levelValue, int? levelRAG)
        {
            var numberOfAffectedRows = connection.Execute(
                             @"INSERT INTO dbo.CompetencyAssessmentQuestionRoleRequirements
                                   (SelfAssessmentID
                                   ,CompetencyID
                                   ,AssessmentQuestionID
                                   ,LevelValue
                                   ,LevelRAG)
                                 VALUES
                                   (@assessmentId
                                   ,@competencyId
                                   ,@assessmentQuestionId
                                   ,@levelValue
                                   ,@levelRAG);"
                         ,
                             new { assessmentId, competencyId, assessmentQuestionId, levelValue, levelRAG }
                         );
            return numberOfAffectedRows;
        }

        public bool DeleteCompetencyAssessmentQuestionRoleRequirement(int assessmentId, int? competencyId, int assessmentQuestionId, int levelValue)
        {
            var numberOfAffectedRows = connection.Execute(
                             @"DELETE FROM CompetencyAssessmentQuestionRoleRequirements
                                  WHERE SelfAssessmentID = @assessmentId
                                    AND AssessmentQuestionID = @assessmentQuestionId
                                    AND LevelValue = @levelValue
                                    AND (CompetencyID = @competencyId OR @competencyId IS NULL);"
                         ,
                             new { assessmentId, competencyId, assessmentQuestionId, levelValue }
                         );
            return numberOfAffectedRows > 0;
        }
        public void InsertIntoSelfAssessmentCollaboratorsFromFrameworkCollaborators(int selfAssessmentId, int? frameworkId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO [dbo].[SelfAssessmentCollaborators]
                  (
                    [SelfAssessmentID],
                    [UserEmail],
                    [AdminID],
                    [CanModify],
                    [IsDeleted])
                    SELECT
                    @selfAssessmentID,
                    fc.UserEmail,
                    fc.AdminID,
                    fc.CanModify,
                    fc.IsDeleted
                    FROM FrameworkCollaborators fc
                    INNER JOIN AdminUsers au
                    ON fc.AdminID = au.AdminID
                    WHERE fc.FrameworkID = @frameworkId
                    AND fc.IsDeleted = 0;",
                new { selfAssessmentId, frameworkId }
            );
        }
        public bool UpdateSupervisorRolesTaskStatus(int competencyAssessmentId, bool taskCompleteChecked)
        {
            var numberOfAffectedRows = connection.Execute(
                        @"  UPDATE SelfAssessmentTaskStatus
            SET SupervisorRolesTaskStatus = CASE WHEN @taskCompleteChecked = 1 THEN 1 ELSE 0 END
            WHERE SelfAssessmentId = @competencyAssessmentId",
                        new { competencyAssessmentId, taskCompleteChecked = taskCompleteChecked ? 1 : 0 }
                    );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating SupervisorRolesTaskStatus as db update failed. " +
                    $"competencyAssessmentId: {competencyAssessmentId}, taskCompleteChecked: {taskCompleteChecked}"
                );
                return false;
            }
            return true;
        }
        public bool UpdateSelfAssessments(int competencyAssessmentId,
            int? supervised,
            int? signoff,
            int? confirm,
            int? supervisorDeclarationValue,
            string? supervisorCustomText,
            int? leanerDeclarationValue,
            string? leanerCustomText
           )
        {
            var sqlQuery = @"
            IF @supervised = 0 
            BEGIN
                UPDATE SelfAssessments SET SupervisorResultsReview = 0,
                SupervisorSelfAssessmentReview = 0,
                    SignOffSupervisorStatement =  NULL,
                    SignOffRequestorStatement = NULL
                WHERE ID = @competencyAssessmentId;
            END
            ELSE IF @supervised = 1 AND @signoff = 0
            BEGIN
                UPDATE SelfAssessments SET SupervisorResultsReview = 0,
                SupervisorSelfAssessmentReview = 0,
                    SignOffSupervisorStatement =  NULL,
                    SignOffRequestorStatement = NULL
                WHERE ID = @competencyAssessmentId;
            END
            ELSE
            BEGIN
                UPDATE SelfAssessments
                SET SupervisorResultsReview = 1,
                    SupervisorSelfAssessmentReview = @confirm,
                    SignOffSupervisorStatement = CASE WHEN @supervisorDeclarationValue = 0 THEN NULL ELSE @supervisorCustomText END,
                    SignOffRequestorStatement = CASE WHEN @leanerDeclarationValue = 0 THEN NULL ELSE @leanerCustomText END
                WHERE ID = @competencyAssessmentId;
            END
        ";
            var affectedRows = connection.Execute(
            sqlQuery,
            new
            {
                competencyAssessmentId,
                supervised,
                confirm,
                signoff,
                supervisorDeclarationValue,
                supervisorCustomText,
                leanerDeclarationValue,
                leanerCustomText
            }
             );

            if ((affectedRows < 1))
            {
                logger.LogWarning(
                    "Not updating SelfAssessments  as db update failed. " +
                    $"competencyAssessmentId: {competencyAssessmentId}, supervised: {supervised}" +
                    $"signoff: {signoff}, confirm: {confirm}, supervisorDeclarationValue: {supervisorDeclarationValue} " +
                    $"supervisorCustomText: {supervisorCustomText}, leanerDeclarationValue: {leanerDeclarationValue}, leanerCustomText: {leanerCustomText} "
                    );
                return false;
            }
            return true;
        }
    }
}
