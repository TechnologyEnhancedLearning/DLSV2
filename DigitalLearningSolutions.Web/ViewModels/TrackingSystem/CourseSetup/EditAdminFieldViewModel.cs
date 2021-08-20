namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditAdminFieldViewModel : AdminFieldAnswersViewModel
    {
        public EditAdminFieldViewModel() { }

        public EditAdminFieldViewModel(CustomPrompt customPrompt, int customisationId)
        {
            CustomisationId = customisationId;
            PromptNumber = customPrompt.CustomPromptNumber;
            Prompt = customPrompt.CustomPromptText;
            Mandatory = customPrompt.Mandatory;
            OptionsString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(customPrompt.Options);
            IncludeAnswersTableCaption = true;
        }

        public EditAdminFieldViewModel(
            int customisationId,
            int customPromptNumber,
            string text,
            string? options,
            bool mandatory
        )
        {
            CustomisationId = customisationId;
            PromptNumber = customPromptNumber;
            Prompt = text;
            Mandatory = mandatory;
            OptionsString = options;
            IncludeAnswersTableCaption = true;
        }

        public int PromptNumber { get; set; }

        public string Prompt { get; set; }

        public bool Mandatory { get; set; }
    }
}
