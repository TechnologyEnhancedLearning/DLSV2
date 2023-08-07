namespace DigitalLearningSolutions.Data.Models.Centres
{
    public class CentreSummaryForRoleLimits
    {
        public int CentreId { get; set; }

        public int? RoleLimitCmsAdministrators { get; set; }

        public int? RoleLimitCmsManagers { get; set; }

        public int? RoleLimitCcLicences { get; set; }

        public int? RoleLimitCustomCourses { get; set; }

        public int? RoleLimitTrainers { get; set; }
    }
}
