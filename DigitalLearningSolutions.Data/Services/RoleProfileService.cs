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
        IEnumerable<RoleProfile> GetAllRoleProfiles(int adminId);
        IEnumerable<RoleProfile> GetRoleProfilesForAdminId(int adminId);
        
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
        private const string RoleProfileFields =
            @"rp.ID, rp.RoleProfileName, rp.Description, rp.CreatedDate, rp.BrandID,
                 (SELECT BrandName
                 FROM    Brands
                 WHERE (BrandID = rp.BrandID)) AS Brand, rp.ParentRoleProfileID,
                 (SELECT RoleProfileName
                 FROM    RoleProfiles AS rp2
                 WHERE (ID = rp.ParentRoleProfileID)) AS ParentRoleProfile, rp.[National], rp.[Public], rp.OwnerAdminID,
                 (SELECT Forename + ' ' + Surname AS Expr1
                 FROM    AdminUsers
                 WHERE (AdminID = rp.OwnerAdminID)) AS Owner, rp.Archived, rp.LastEdit, rp.NRPProfessionalGroupID,
                 (SELECT ProfessionalGroup
                 FROM    NRPProfessionalGroups
                 WHERE (ID = rp.NRPProfessionalGroupID)) AS NRPProfessionalGroup, rp.NRPSubGroupID,
                 (SELECT SubGroup
                 FROM    NRPSubGroups
                 WHERE (ID = rp.NRPSubGroupID)) AS NRPSubGroup, rp.NRPRoleID,
                 (SELECT RoleProfile
                 FROM    NRPRoles
                 WHERE (ID = rp.NRPRoleID)) AS NRPRole, rp.PublishStatusID, CASE WHEN rp.OwnerAdminID = @adminId THEN 3 WHEN rpc.CanModify = 1 THEN 2 WHEN rpc.CanModify = 0 THEN 1 ELSE 0 END AS UserRole, rpr.ID AS RoleProfileReviewID";
        private const string RoleProfileTables =
            @"RoleProfiles AS rp LEFT OUTER JOIN
             RoleProfileCollaborators AS rpc ON rpc.RoleProfileID = rp.ID AND rpc.AdminID = @adminId LEFT OUTER JOIN
             RoleProfileReviews AS rpr ON rpc.ID = rpr.RoleProfileCollaboratorID AND rpr.Archived IS NULL AND rpr.ReviewComplete IS NULL";
        public IEnumerable<RoleProfile> GetAllRoleProfiles(int adminId)
        {
            return connection.Query<RoleProfile>(
               $@"SELECT {RoleProfileFields}
                      FROM {RoleProfileTables}", new { adminId }
          );
        }

        public IEnumerable<RoleProfile> GetRoleProfilesForAdminId(int adminId)
        {
            return connection.Query<RoleProfile>(
                $@"SELECT {RoleProfileFields}
                      FROM {RoleProfileTables}
                      WHERE (rp.OwnerAdminID = @adminId) OR
             (@adminId IN
                 (SELECT AdminID
                 FROM    RoleProfileCollaborators
                 WHERE (RoleProfileID = rp.ID)))",
               new { adminId }
           );
        }

    }
}
