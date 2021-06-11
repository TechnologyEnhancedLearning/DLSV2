namespace DigitalLearningSolutions.Web.ViewModels.RoleProfiles
{
    public class RoleProfilesViewModel
    {
        public RoleProfilesViewModel(bool isWorkforceManager, bool isWorkforceContributor, AllRoleProfilesViewModel allRoleProfiles, MyRoleProfilesViewModel myRoleProfiles)
        {
            IsWorkforceManager = isWorkforceManager;
            IsWorkforceContributor = isWorkforceContributor;
        }

        public bool IsWorkforceManager { get; set; }
        public bool IsWorkforceContributor { get; set; }
        public MyRoleProfilesViewModel MyRoleProfilesViewModel { get; set; }
        public AllRoleProfilesViewModel AllRoleProfilesViewModel { get; set; }
    }
}
