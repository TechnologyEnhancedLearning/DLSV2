namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.ComponentModel.DataAnnotations;

    public class AddDelegateGroupViewModel
    {
        [Required(ErrorMessage = "Enter a group name")]
        [MaxLength(100, ErrorMessage = "Group name must be 100 characters or fewer")]
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
    }
}
