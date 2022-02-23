namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditAdminFieldViewModel : AdminFieldAnswersViewModel
    {
        public EditAdminFieldViewModel() { }

        public EditAdminFieldViewModel(CustomPrompt customPrompt)
        {
            PromptNumber = customPrompt.CustomPromptNumber;
            Prompt = customPrompt.CustomPromptText;
            OptionsString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(customPrompt.Options);
            IncludeAnswersTableCaption = true;
        }

        public EditAdminFieldViewModel(
            int customPromptNumber,
            string text,
            string? options
        )
        {
            PromptNumber = customPromptNumber;
            Prompt = text;
            OptionsString = options;
            IncludeAnswersTableCaption = true;
        }

        public int PromptNumber { get; set; }

        public string Prompt { get; set; }
    }
}
