namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditAdminFieldViewModel : AdminFieldAnswersViewModel
    {
        public EditAdminFieldViewModel()
        {
            IncludeAnswersTableCaption = true;
        }

        public EditAdminFieldViewModel(
            int customisationId,
            int promptNumber,
            string prompt,
            bool mandatory,
            string optionsString,
            string? answer = null
        )
            : base(optionsString, answer, true)
        {
            CustomisationId = customisationId;
            PromptNumber = promptNumber;
            Prompt = prompt;
            Mandatory = mandatory;
        }

        public EditAdminFieldViewModel(CustomPrompt customPrompt)
        {
            PromptNumber = customPrompt.CustomPromptNumber;
            Prompt = customPrompt.CustomPromptText;
            Mandatory = customPrompt.Mandatory;
            OptionsString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(customPrompt.Options);
            IncludeAnswersTableCaption = true;
        }

        public int CustomisationId { get; set; }

        public int PromptNumber { get; set; }

        public string Prompt { get; set; }

        public bool Mandatory { get; set; }
    }
}
