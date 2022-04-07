namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System.Collections.Generic;

    public class RoleProfilesTab : BaseTabEnumeration
    {
        public static RoleProfilesTab MyRoleProfiles = new RoleProfilesTab(
            1,
            nameof(MyRoleProfiles),
            "RoleProfiles",
            "ViewRoleProfiles",
            "My Role Profiles",
            new Dictionary<string, string> { { "tabName", "Mine" } }
        );

        public static RoleProfilesTab AllRoleProfiles = new RoleProfilesTab(
            2,
            nameof(AllRoleProfiles),
            "RoleProfiles",
            "ViewRoleProfiles",
            "All Role Profiles",
            new Dictionary<string, string> { { "tabName", "All" } }
        );

        private RoleProfilesTab(
            int id,
            string name,
            string controller,
            string action,
            string linkText,
            Dictionary<string, string> staticRouteData
        ) : base(id, name, controller, action, linkText, staticRouteData) { }

        public override IEnumerable<BaseTabEnumeration> GetAllTabs()
        {
            return GetAll<RoleProfilesTab>();
        }
    }
}
