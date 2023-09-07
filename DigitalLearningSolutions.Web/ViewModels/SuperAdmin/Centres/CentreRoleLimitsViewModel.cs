namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using System.ComponentModel.DataAnnotations;

    public class CentreRoleLimitsViewModel
    {
        public int CentreId { get; set; }
        public bool IsRoleLimitSetCmsAdministrators { get; set; }
        public bool IsRoleLimitSetCmsManagers { get; set; }
        public bool IsRoleLimitSetContentCreatorLicences { get; set; }
        public bool IsRoleLimitSetCustomCourses { get; set; }
        public bool IsRoleLimitSetTrainers { get; set; }

        [Required(ErrorMessage = "The role limit is required.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a number.")]
        public int RoleLimitCmsAdministrators { get; set; }

        [Required(ErrorMessage = "The role limit is required.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a number.")]
        public int RoleLimitCmsManagers { get; set; }

        [Required(ErrorMessage = "The role limit is required.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a number.")]
        public int RoleLimitContentCreatorLicences { get; set; }

        [Required(ErrorMessage = "The role limit is required.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a number.")]
        public int RoleLimitCustomCourses { get; set; }

        [Required(ErrorMessage = "The role limit is required.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a number.")]
        public int RoleLimitTrainers { get; set; }
    }
}
