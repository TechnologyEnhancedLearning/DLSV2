namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using System.ComponentModel.DataAnnotations;

    public class CentreRoleLimitsViewModel
    {
        public int CentreId { get; set; }
        public bool IsRoleLimitSetCmsAdministrators { get; set; }
        public bool IsRoleLimitSetCmsManagers { get; set; }
        public bool IsRoleLimitSetContentCreatorLicenses { get; set; }
        public bool IsRoleLimitSetCustomCourses { get; set; }
        public bool IsRoleLimitSetTrainers { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int RoleLimitCmsAdministrators { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int RoleLimitCmsManagers { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int RoleLimitContentCreatorLicenses { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int RoleLimitCustomCourses { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int RoleLimitTrainers { get; set; }
    }
}
