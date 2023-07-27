namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.Centres;

    public class CentreRoleLimitsViewModel
    {
        public int CentreId { get; set; }
        public bool IsLimitSetCMSAdministrators { get; set; }
        public int LimitSetCMSAdministrators { get; set; }
        public bool IsLimitSetCMSManagers { get; set; }
        public int LimitSetCMSManagers { get; set; }
        public bool IsLimitSetContentCreatorLicenses { get; set; }
        public int LimitSetContentCreatorLicenses { get; set; }
        public bool IsLimitSetCustomCourses { get; set; }
        public int LimitSetCustomCourses { get; set; }
        public bool IsLimitSetTrainers { get; set; }
        public int LimitSetTrainers { get; set; }
    }
}
