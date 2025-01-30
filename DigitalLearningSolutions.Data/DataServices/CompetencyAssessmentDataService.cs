﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DocumentFormat.OpenXml.Wordprocessing;
    using Microsoft.Extensions.Logging;

    public interface ICompetencyAssessmentDataService
    {
        //GET DATA
        IEnumerable<CompetencyAssessment> GetAllCompetencyAssessments(int adminId);

        IEnumerable<CompetencyAssessment> GetCompetencyAssessmentsForAdminId(int adminId);

        CompetencyAssessmentBase? GetCompetencyAssessmentBaseById(int competencyAssessmentId, int adminId);

        CompetencyAssessmentBase? GetCompetencyAssessmentByName(string competencyAssessmentName, int adminId);

        IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups();

        //UPDATE DATA
        bool UpdateCompetencyAssessmentName(int competencyAssessmentId, int adminId, string competencyAssessmentName);

        bool UpdateCompetencyAssessmentProfessionalGroup(int competencyAssessmentId, int adminId, int? nrpProfessionalGroupID);
        bool UpdateCompetencyAssessmentBranding(
            int competencyAssessmentId,
            int brandId,
            int categoryId,
            int adminId
        );
        bool UpdateCompetencyAssessmentDescription(int competencyAssessmentId, int adminId, string competencyAssessmentDescription);
        //INSERT DATA
        int InsertCompetencyAssessment(int adminId, int centreId, string competencyAssessmentName);
        bool InsertSelfAssessmentFramework(int adminId, int selfAssessmentId, int frameworkId);
        //DELETE DATA
    }

    public class CompetencyAssessmentDataService : ICompetencyAssessmentDataService
    {
        private const string SelfAssessmentBaseFields = @"sa.ID, sa.Name AS CompetencyAssessmentName, sa.Description, sa.BrandID,
                sa.ParentSelfAssessmentID,
                sa.[National], sa.[Public], sa.CreatedByAdminID AS OwnerAdminID,
                sa.NRPProfessionalGroupID,
                 sa.NRPSubGroupID,
                 sa.NRPRoleID,
                 sa.PublishStatusID, CASE WHEN sa.CreatedByAdminID = @adminId THEN 3 WHEN sac.CanModify = 1 THEN 2 WHEN sac.CanModify = 0 THEN 1 ELSE 0 END AS UserRole";

        private const string SelfAssessmentFields =
            @", sa.CreatedDate,
                 (SELECT BrandName
                 FROM    Brands
                 WHERE (BrandID = sa.BrandID)) AS Brand,
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
                            Frameworks f
                       WHERE 
                            f.ID = saf.FrameworkId
                        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS LinkedFrameworks,
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
             SelfAssessmentReviews AS sar ON sac.ID = sar.SelfAssessmentCollaboratorID AND sar.Archived IS NULL AND sar.ReviewComplete IS NULL
                LEFT OUTER JOIN SelfAssessmentFrameworks AS saf ON sa.ID = saf.SelfAssessmentId";

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

        public CompetencyAssessmentBase? GetCompetencyAssessmentByName(string competencyAssessmentName, int adminId)
        {
            return connection.Query<CompetencyAssessmentBase>(
                $@"SELECT {SelfAssessmentBaseFields}
                      FROM {SelfAssessmentBaseTables}
                      WHERE (sa.Name = @competencyAssessmentName)",
                new { competencyAssessmentName, adminId }
            ).FirstOrDefault();
        }

        public bool UpdateCompetencyAssessmentProfessionalGroup(int competencyAssessmentId, int adminId, int? nrpProfessionalGroupID)
        {
            var result = connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM CompetencyAssessments WHERE ID = @competencyAssessmentId AND NRPProfessionalGroupID = @nrpProfessionalGroupID",
                new { competencyAssessmentId, nrpProfessionalGroupID }
            );
            int sameCount = Convert.ToInt32(result);
            if (sameCount > 0)
            {
                //same so don't update:
                return false;
            }

            //needs updating:
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessments SET NRPProfessionalGroupID = @nrpProfessionalGroupID, NRPSubGroupID = NULL, NRPRoleID = NULL, UpdatedByAdminID = @adminId
                    WHERE ID = @competencyAssessmentId",
                new { nrpProfessionalGroupID, adminId, competencyAssessmentId }
            );
            if (numberOfAffectedRows > 0)
            {
                return true;
            }

            return false;
        }
        public bool UpdateCompetencyAssessmentBranding(
            int competencyAssessmentId,
            int brandId,
            int categoryId,
            int adminId
        )
        {
            if ((competencyAssessmentId < 1) | (brandId < 1) | (categoryId < 1)  | (adminId < 1))
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
                    "Not updating competency assessment as db update failed. " +
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
                    "Not updating competency assessment as db update failed. " +
                    $"frameworkId: {competencyAssessmentId}, competencyAssessmentDescription: {competencyAssessmentDescription}, AdminId: {adminId}"
                );
                return false;
            }
            return true;
        }

        public bool InsertSelfAssessmentFramework(int adminId, int selfAssessmentId, int frameworkId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO SelfAssessmentFrameworks (SelfAssessmentId, FrameworkId, CreatedByAdminId)
                    SELECT @selfAssessmentId, @frameworkId, @adminId
                    WHERE NOT EXISTS (SELECT 1 FROM SelfAssessmentFrameworks WHERE SelfAssessmentId = @selfAssessmentId AND FrameworkId = @frameworkId)"
            ,
                new { adminId, selfAssessmentId, frameworkId }
            );
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
    }
}
