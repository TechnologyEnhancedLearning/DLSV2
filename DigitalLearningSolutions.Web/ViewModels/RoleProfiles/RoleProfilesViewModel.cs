namespace DigitalLearningSolutions.Web.ViewModels.RoleProfiles
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class RoleProfilesViewModel
    {
        public RoleProfilesViewModel(bool isWorkforceManager, bool isWorkforceContributor, AllRoleProfilesViewModel allRoleProfiles, MyRoleProfilesViewModel myRoleProfiles, RoleProfilesTab currentTab)
        {
            IsWorkforceManager = isWorkforceManager;
            IsWorkforceContributor = isWorkforceContributor;
            MyRoleProfilesViewModel = myRoleProfiles;
            AllRoleProfilesViewModel = allRoleProfiles;
            TabNavLinks = new TabsNavViewModel(currentTab);
        }

        public bool IsWorkforceManager { get; set; }
        public bool IsWorkforceContributor { get; set; }
        public MyRoleProfilesViewModel MyRoleProfilesViewModel { get; set; }
        public AllRoleProfilesViewModel AllRoleProfilesViewModel { get; set; }
        public TabsNavViewModel TabNavLinks { get; set; }
    }
}
