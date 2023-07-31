namespace DigitalLearningSolutions.Data.Models.Centres
{
    public class CentreSummaryForRoleLimits
    {
        public int CentreId { get; set; }

        public int RoleLimitCMSAdministrators { get; set; }

        public int RoleLimitCMSManagers { get; set; }

        public int RoleLimitCCLicenses { get; set; }

        public int RoleLimitCustomCourses { get; set; }

        public int RoleLimitTrainers { get; set; }
    }
}
