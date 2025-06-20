namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.Frameworks.Import;
    using DocumentFormat.OpenXml.Wordprocessing;
    using Microsoft.Extensions.Logging;

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

        int[] GetLinkedFrameworkIds (int assessmentId);

        int? GetPrimaryLinkedFrameworkId(int assessmentId);

        int GetCompetencyCountByFrameworkId(int assessmentId, int frameworkId);

        IEnumerable<Competency> GetCompetenciesForCompetencyAssessment(int competencyAssessmentId);
        IEnumerable<BaseFramework> GetLinkedFrameworksForCompetencyAssessment(int competencyAssessmentId);

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

        //INSERT DATA
        int InsertCompetencyAssessment(int adminId, int centreId, string competencyAssessmentName);
        bool InsertSelfAssessmentFramework(int adminId, int selfAssessmentId, int frameworkId);

        //DELETE DATA
        bool RemoveFrameworkCompetenciesFromAssessment(int competencyAssessmentId, int frameworkId);
        
    }

    public class CompetencyAssessmentDataService : ICompetencyAssessmentDataService
    {
        private const string SelfAssessmentBaseFields = @"sa.ID, sa.Name AS CompetencyAssessmentName, sa.Description, sa.BrandID,
                sa.ParentSelfAssessmentID,
                sa.[National], sa.[Public], sa.CreatedByAdminID AS OwnerAdminID,
                sa.NRPProfessionalGroupID,
                 sa.NRPSubGroupID,
                 sa.NRPRoleID,
                 sa.PublishStatusID, sa.Vocabulary, CASE WHEN sa.CreatedByAdminID = @adminId THEN 3 WHEN sac.CanModify = 1 THEN 2 WHEN sac.CanModify = 0 THEN 1 ELSE 0 END AS UserRole";

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
                        saf2.SelfAssessmentId = sa.ID
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
                    SET RemovedDate = NULL, RemovedByAdminId = NULL, AmendedByAdminId = @adminId
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

        public IEnumerable<Competency> GetCompetenciesForCompetencyAssessment(int competencyAssessmentId)
        {
            return connection.Query<Competency>(
               @"SELECT sas.ID AS StructureId, sas.CompetencyID, f.FrameworkName, cg.Name AS GroupName, c.Name AS CompetencyName, c.Description AS CompetencyDescription, sas.Optional
                    FROM   SelfAssessmentStructure AS sas INNER JOIN
                                Competencies AS c ON sas.CompetencyID = c.ID INNER JOIN
                                CompetencyGroups AS cg ON sas.CompetencyGroupID = cg.ID INNER JOIN
                                FrameworkCompetencies ON c.ID = FrameworkCompetencies.CompetencyID INNER JOIN
                                Frameworks AS f ON FrameworkCompetencies.FrameworkID = f.ID INNER JOIN
                                SelfAssessmentFrameworks ON f.ID = SelfAssessmentFrameworks.FrameworkId AND sas.SelfAssessmentID = SelfAssessmentFrameworks.SelfAssessmentId
                    WHERE (sas.SelfAssessmentID = @competencyAssessmentId)
                    ORDER BY sas.Ordering", new { competencyAssessmentId }
           );
        }

        public IEnumerable<BaseFramework> GetLinkedFrameworksForCompetencyAssessment(int competencyAssessmentId)
        {
            return connection.Query<BaseFramework>(
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
    }
}
