namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;
    using DigitalLearningSolutions.Data.Models.Email;
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
        private const string RoleProfileBaseFields = @"rp.ID, rp.RoleProfileName, rp.Description, rp.BrandID,
                rp.ParentRoleProfileID,
                rp.[National], rp.[Public], rp.OwnerAdminID,
                rp.NRPProfessionalGroupID,
                 rp.NRPSubGroupID,
                 rp.NRPRoleID,
                 rp.PublishStatusID, CASE WHEN rp.OwnerAdminID = @adminId THEN 3 WHEN rpc.CanModify = 1 THEN 2 WHEN rpc.CanModify = 0 THEN 1 ELSE 0 END AS UserRole";
        private const string RoleProfileFields =
            @", rp.CreatedDate,
                 (SELECT BrandName
                 FROM    Brands
                 WHERE (BrandID = rp.BrandID)) AS Brand, 
                 (SELECT RoleProfileName
                 FROM    RoleProfiles AS rp2
                 WHERE (ID = rp.ParentRoleProfileID)) AS ParentRoleProfile, 
                 (SELECT Forename + ' ' + Surname AS Expr1
                 FROM    AdminUsers
                 WHERE (AdminID = rp.OwnerAdminID)) AS Owner, rp.Archived, rp.LastEdit, 
                 (SELECT ProfessionalGroup
                 FROM    NRPProfessionalGroups
                 WHERE (ID = rp.NRPProfessionalGroupID)) AS NRPProfessionalGroup, 
                 (SELECT SubGroup
                 FROM    NRPSubGroups
                 WHERE (ID = rp.NRPSubGroupID)) AS NRPSubGroup, 
                 (SELECT RoleProfile
                 FROM    NRPRoles
                 WHERE (ID = rp.NRPRoleID)) AS NRPRole, rpr.ID AS RoleProfileReviewID";
        private const string RoleProfileBaseTables =
            @"RoleProfiles AS rp LEFT OUTER JOIN
             RoleProfileCollaborators AS rpc ON rpc.RoleProfileID = rp.ID AND rpc.AdminID = @adminId";
        private const string RoleProfileTables =
            @" LEFT OUTER JOIN
             RoleProfileReviews AS rpr ON rpc.ID = rpr.RoleProfileCollaboratorID AND rpr.Archived IS NULL AND rpr.ReviewComplete IS NULL";
        public IEnumerable<RoleProfile> GetAllRoleProfiles(int adminId)
        {
            return connection.Query<RoleProfile>(
               $@"SELECT {RoleProfileBaseFields} {RoleProfileFields}
                      FROM {RoleProfileBaseTables} {RoleProfileTables}", new { adminId }
          );
        }

        public IEnumerable<RoleProfile> GetRoleProfilesForAdminId(int adminId)
        {
            return connection.Query<RoleProfile>(
                $@"SELECT {RoleProfileBaseFields} {RoleProfileFields}
                      FROM {RoleProfileBaseTables} {RoleProfileTables}
                      WHERE (rp.OwnerAdminID = @adminId) OR
             (@adminId IN
                 (SELECT AdminID
                 FROM    RoleProfileCollaborators
                 WHERE (RoleProfileID = rp.ID)))",
               new { adminId }
           );
        }

        public RoleProfileBase GetRoleProfileBaseById(int roleProfileId, int adminId)
        {
            return connection.Query<RoleProfileBase>(
               $@"SELECT {RoleProfileBaseFields} 
                      FROM {RoleProfileBaseTables}
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
            int existingRoleProfiles = (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM RoleProfiles WHERE RoleProfileName = @roleProfileName AND ID <> @roleProfileId",
                new { roleProfileName, roleProfileId });
            if (existingRoleProfiles > 0)
            {
                return false;
            }
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE RoleProfiles SET RoleProfileName = @roleProfileName, UpdatedByAdminID = @adminId
                    WHERE ID = @roleProfileId",
               new { roleProfileName, adminId, roleProfileId }
           );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating role profile name as db update failed. " +
                    $"RoleProfileName: {roleProfileName}, admin id: {adminId}, roleProfileId: {roleProfileId}"
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
               $@"SELECT {RoleProfileBaseFields} 
                      FROM {RoleProfileBaseTables}
                      WHERE (rp.RoleProfileName = @roleProfileName)",
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
               @"UPDATE RoleProfiles SET NRPProfessionalGroupID = @nrpProfessionalGroupID, NRPSubGroupID = NULL, NRPRoleID = NULL, UpdatedByAdminID = @adminId
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
