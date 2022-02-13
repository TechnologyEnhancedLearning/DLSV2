namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using Microsoft.Extensions.Logging;

    public interface IRoleProfileService
    {
        //GET DATA
        IEnumerable<RoleProfile> GetAllRoleProfiles(int adminId);
        IEnumerable<RoleProfile> GetRoleProfilesForAdminId(int adminId);
        RoleProfileBase GetRoleProfileBaseById(int roleProfileId, int adminId);
        RoleProfileBase? GetRoleProfileByName(string roleProfileName, int adminId);
        IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups();
        //UPDATE DATA
        bool UpdateRoleProfileName(int roleProfileId, int adminId, string roleProfileName);
        bool UpdateRoleProfileProfessionalGroup(int roleProfileId, int adminId, int? nrpProfessionalGroupID);
        //INSERT DATA

        //DELETE DATA


    }
    public class RoleProfileService : IRoleProfileService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<RoleProfileService> logger;
        public RoleProfileService(IDbConnection connection, ILogger<RoleProfileService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }
        private const string SelfAssessmentBaseFields = @"rp.ID, rp.Name AS RoleProfileName, rp.Description, rp.BrandID,
                rp.ParentSelfAssessmentID,
                rp.[National], rp.[Public], rp.CreatedByAdminID AS OwnerAdminID,
                rp.NRPProfessionalGroupID,
                 rp.NRPSubGroupID,
                 rp.NRPRoleID,
                 rp.PublishStatusID, CASE WHEN rp.CreatedByAdminID = @adminId THEN 3 WHEN rpc.CanModify = 1 THEN 2 WHEN rpc.CanModify = 0 THEN 1 ELSE 0 END AS UserRole";
        private const string SelfAssessmentFields =
            @", rp.CreatedDate,
                 (SELECT BrandName
                 FROM    Brands
                 WHERE (BrandID = rp.BrandID)) AS Brand, 
                 (SELECT [Name]
                 FROM    SelfAssessments AS rp2
                 WHERE (ID = rp.ParentSelfAssessmentID)) AS ParentSelfAssessment, 
                 (SELECT CASE WHEN Active = 1 THEN Forename + ' ' + Surname ELSE '(Inactive)' END AS Expr1
                 FROM    AdminUsers
                 WHERE (AdminID = rp.CreatedByAdminID)) AS Owner,
                 rp.Archived,
                 rp.LastEdit, 
                 (SELECT ProfessionalGroup
                 FROM    NRPProfessionalGroups
                 WHERE (ID = rp.NRPProfessionalGroupID)) AS NRPProfessionalGroup, 
                 (SELECT SubGroup
                 FROM    NRPSubGroups
                 WHERE (ID = rp.NRPSubGroupID)) AS NRPSubGroup, 
                 (SELECT RoleProfile
                 FROM    NRPRoles
                 WHERE (ID = rp.NRPRoleID)) AS NRPRole, rpr.ID AS SelfAssessmentReviewID";
        private const string SelfAssessmentBaseTables =
            @"SelfAssessments AS rp LEFT OUTER JOIN
             SelfAssessmentCollaborators AS rpc ON rpc.SelfAssessmentID = rp.ID AND rpc.AdminID = @adminId";
        private const string SelfAssessmentTables =
            @" LEFT OUTER JOIN
             SelfAssessmentReviews AS rpr ON rpc.ID = rpr.SelfAssessmentCollaboratorID AND rpr.Archived IS NULL AND rpr.ReviewComplete IS NULL";
        public IEnumerable<RoleProfile> GetAllRoleProfiles(int adminId)
        {
            return connection.Query<RoleProfile>(
               $@"SELECT {SelfAssessmentBaseFields} {SelfAssessmentFields}
                      FROM {SelfAssessmentBaseTables} {SelfAssessmentTables}", new { adminId }
          );
        }

        public IEnumerable<RoleProfile> GetRoleProfilesForAdminId(int adminId)
        {
            return connection.Query<RoleProfile>(
                $@"SELECT {SelfAssessmentBaseFields} {SelfAssessmentFields}
                      FROM {SelfAssessmentBaseTables} {SelfAssessmentTables}
                      WHERE (rp.CreatedByAdminID = @adminId) OR
             (@adminId IN
                 (SELECT AdminID
                 FROM    SelfAssessmentCollaborators
                 WHERE (SelfAssessmentID = rp.ID)))",
               new { adminId }
           );
        }
        public RoleProfileBase GetRoleProfileBaseById(int roleProfileId, int adminId)
        {
            return connection.Query<RoleProfileBase>(
               $@"SELECT {SelfAssessmentBaseFields} 
                      FROM {SelfAssessmentBaseTables}
                      WHERE (rp.ID = @roleProfileId)",
              new { roleProfileId, adminId }
          ).FirstOrDefault();
        }

        public bool UpdateRoleProfileName(int roleProfileId, int adminId, string roleProfileName)
        {
            if (roleProfileName.Length == 0 | adminId < 1 | roleProfileId < 1)
            {
                logger.LogWarning(
                    $"Not updating role profile name as it failed server side validation. AdminId: {adminId}, roleProfileName: {roleProfileName}, roleProfileId: {roleProfileId}"
                );
                return false;
            }
            int existingSelfAssessments = (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM SelfAssessments WHERE [Name] = @roleProfileName AND ID <> @roleProfileId",
                new { roleProfileName, roleProfileId });
            if (existingSelfAssessments > 0)
            {
                return false;
            }
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessments SET [Name] = @roleProfileName, UpdatedByAdminID = @adminId
                    WHERE ID = @roleProfileId",
               new { roleProfileName, adminId, roleProfileId }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating role profile name as db update failed. " +
                    $"SelfAssessmentName: {roleProfileName}, admin id: {adminId}, roleProfileId: {roleProfileId}"
                );
                return false;
            }
            else
            {
                return true;
            }
        }

        public IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups()
        {
            return connection.Query<NRPProfessionalGroups>(
                $@"SELECT ID, ProfessionalGroup, Active
                    FROM   NRPProfessionalGroups
                    WHERE (Active = 1)
                    ORDER BY ProfessionalGroup"
           );
        }

        public RoleProfileBase? GetRoleProfileByName(string roleProfileName, int adminId)
        {
            return connection.Query<RoleProfileBase>(
               $@"SELECT {SelfAssessmentBaseFields} 
                      FROM {SelfAssessmentBaseTables}
                      WHERE (rp.Name = @roleProfileName)",
              new { roleProfileName, adminId }
          ).FirstOrDefault();
        }

        public bool UpdateRoleProfileProfessionalGroup(int roleProfileId, int adminId, int? nrpProfessionalGroupID)
        {
            var sameCount = (int)connection.ExecuteScalar(
                 @"SELECT COUNT(*) FROM RoleProfiles WHERE ID = @roleProfileId AND NRPProfessionalGroupID = @nrpProfessionalGroupID",
                 new { roleProfileId, nrpProfessionalGroupID }
                );
            if (sameCount > 0)
            {
                //same so don't update:
                return false;
            }
            else
            {
                //needs updating:
                var numberOfAffectedRows = connection.Execute(
               @"UPDATE SelfAssessments SET NRPProfessionalGroupID = @nrpProfessionalGroupID, NRPSubGroupID = NULL, NRPRoleID = NULL, UpdatedByAdminID = @adminId
                    WHERE ID = @roleProfileId",
              new { nrpProfessionalGroupID, adminId, roleProfileId }
              );
                if(numberOfAffectedRows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
