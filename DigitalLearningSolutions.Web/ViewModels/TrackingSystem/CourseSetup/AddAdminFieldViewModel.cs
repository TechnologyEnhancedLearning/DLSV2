namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.ComponentModel.DataAnnotations;

    public class AddAdminFieldViewModel : AdminFieldAnswersViewModel
    {
        public AddAdminFieldViewModel()
        {
            IncludeAnswersTableCaption = true;
        }

        public AddAdminFieldViewModel(int customisationId)
        {
            CustomisationId = customisationId;
            IncludeAnswersTableCaption = true;
        }

        public AddAdminFieldViewModel(
            int customisationId,
            int customPromptId,
            int customPromptNumber
        )
        {
            CustomisationId = customisationId;
            CustomPromptId = customPromptId;
            PromptNumber = customPromptNumber;
            IncludeAnswersTableCaption = true;
        }

        public int PromptNumber { get; set; }

        [Required(ErrorMessage = "Select a prompt name")]
        public int? CustomPromptId { get; set; }
    }
}
