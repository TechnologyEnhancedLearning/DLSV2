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

        [Required(ErrorMessage = "Please enter a number for the CMS Administrators role limit.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int? RoleLimitCmsAdministrators { get; set; }

        [Required(ErrorMessage = "Please enter a number for the CMS Managers role limit.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int? RoleLimitCmsManagers { get; set; }

        [Required(ErrorMessage = "Please enter a number for the Content Creator Licences role limit.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int? RoleLimitContentCreatorLicences { get; set; }

        [Required(ErrorMessage = "Please enter a number for the Custom Courses role limit.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int? RoleLimitCustomCourses { get; set; }

        [Required(ErrorMessage = "Please enter a number for the Trainers role limit.")]
        [Range(-1, int.MaxValue, ErrorMessage = "The role limit must be a non-negative whole number.")]
        public int? RoleLimitTrainers { get; set; }
    }
}
