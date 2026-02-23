using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class ChangeOwnerViewModel
    {
        public int FrameworkId { get; set; }
        public string? FrameworkName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "New Owner Email")]
        public string NewOwnerEmail { get; set; }
    }
}
