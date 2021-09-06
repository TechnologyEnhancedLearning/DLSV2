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
            string? options,
            string? answer = null
        )
        {
            CustomisationId = customisationId;
            CustomPromptId = customPromptId;
            OptionsString = options;
            IncludeAnswersTableCaption = true;
            Answer = answer;
        }

        [Required(ErrorMessage = "Select a prompt name")]

        public int? CustomPromptId { get; set; }
    }
}
