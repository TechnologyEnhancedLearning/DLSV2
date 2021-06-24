namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using System.ComponentModel.DataAnnotations;

    public class AddRegistrationPromptSelectPromptViewModel
    {
        public AddRegistrationPromptSelectPromptViewModel() { }

        public AddRegistrationPromptSelectPromptViewModel(int customPromptId, bool mandatory)
        {
            CustomPromptId = customPromptId;
            Mandatory = mandatory;
        }

        [Required(ErrorMessage = "Select a prompt name")]
        public int? CustomPromptId { get; set; }

        public bool Mandatory { get; set; }
    }
}
