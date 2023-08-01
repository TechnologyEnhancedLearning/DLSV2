namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.Centres;

    public class CentreRoleLimitsViewModel
    {
        public int CentreId { get; set; }
        public bool? IsRoleLimitSetCmsAdministrators { get; set; }
        public int RoleLimitCmsAdministrators { get; set; }
        public bool? IsRoleLimitSetCmsManagers { get; set; }
        public int RoleLimitCmsManagers { get; set; }
        public bool? IsRoleLimitSetContentCreatorLicenses { get; set; }
        public int RoleLimitContentCreatorLicenses { get; set; }
        public bool? IsRoleLimitSetCustomCourses { get; set; }
        public int RoleLimitCustomCourses { get; set; }
        public bool? IsRoleLimitSetTrainers { get; set; }
        public int RoleLimitTrainers { get; set; }
    }
}
