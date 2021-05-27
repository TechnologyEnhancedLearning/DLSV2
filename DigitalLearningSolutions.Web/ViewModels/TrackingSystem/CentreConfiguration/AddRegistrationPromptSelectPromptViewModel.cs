namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using System.ComponentModel.DataAnnotations;

    public class AddRegistrationPromptSelectPromptViewModel
    {
        [Required]
        public int? CustomPromptId { get; set; }

        public bool Mandatory { get; set; }
    }
}
