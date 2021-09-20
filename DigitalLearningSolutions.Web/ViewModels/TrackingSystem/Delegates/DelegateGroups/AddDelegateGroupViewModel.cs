namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.ComponentModel.DataAnnotations;

    public class AddDelegateGroupViewModel
    {
        [Required(ErrorMessage = "Enter a group name")]
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
    }
}
