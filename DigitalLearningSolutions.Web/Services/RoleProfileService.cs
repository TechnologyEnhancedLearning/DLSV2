using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.RoleProfiles;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IRoleProfileService
    {
        //GET DATA
        IEnumerable<RoleProfile> GetAllRoleProfiles(int adminId);

        IEnumerable<RoleProfile> GetRoleProfilesForAdminId(int adminId);

        RoleProfileBase? GetRoleProfileBaseById(int roleProfileId, int adminId);

        RoleProfileBase? GetRoleProfileByName(string roleProfileName, int adminId);

        IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups();

        //UPDATE DATA
        bool UpdateRoleProfileName(int roleProfileId, int adminId, string roleProfileName);

        bool UpdateRoleProfileProfessionalGroup(int roleProfileId, int adminId, int? nrpProfessionalGroupID);

    }
    public class RoleProfileService : IRoleProfileService
    {
        private readonly IRoleProfileDataService roleProfileDataService;
        public RoleProfileService(IRoleProfileDataService roleProfileDataService)
        {
            this.roleProfileDataService = roleProfileDataService;
        }
        public IEnumerable<RoleProfile> GetAllRoleProfiles(int adminId)
        {
            return roleProfileDataService.GetAllRoleProfiles(adminId);
        }

        public IEnumerable<NRPProfessionalGroups> GetNRPProfessionalGroups()
        {
            return roleProfileDataService.GetNRPProfessionalGroups();
        }

        public RoleProfileBase? GetRoleProfileBaseById(int roleProfileId, int adminId)
        {
            return roleProfileDataService.GetRoleProfileBaseById(roleProfileId, adminId);
        }

        public RoleProfileBase? GetRoleProfileByName(string roleProfileName, int adminId)
        {
            return roleProfileDataService.GetRoleProfileByName(roleProfileName, adminId);
        }

        public IEnumerable<RoleProfile> GetRoleProfilesForAdminId(int adminId)
        {
            return roleProfileDataService.GetRoleProfilesForAdminId(adminId);
        }

        public bool UpdateRoleProfileName(int roleProfileId, int adminId, string roleProfileName)
        {
            return roleProfileDataService.UpdateRoleProfileName(roleProfileId, adminId, roleProfileName);
        }

        public bool UpdateRoleProfileProfessionalGroup(int roleProfileId, int adminId, int? nrpProfessionalGroupID)
        {
            return roleProfileDataService.UpdateRoleProfileProfessionalGroup(roleProfileId, adminId, nrpProfessionalGroupID);
        }
    }
}
