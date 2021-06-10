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
        public IEnumerable<RoleProfile> GetAllRoleProfiles(int adminId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<RoleProfile> GetRoleProfilesForAdminId(int adminId)
        {
            throw new System.NotImplementedException();
        }
    }
}
