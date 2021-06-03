namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditRegistrationPromptViewModel : RegistrationPromptAnswersViewModel
    {
        public EditRegistrationPromptViewModel() { }

        public EditRegistrationPromptViewModel
            (int promptNumber, string prompt, bool mandatory, string optionsString, string? answer = null)
            : base(optionsString, answer)
        {
            PromptNumber = promptNumber;
            Prompt = prompt;
            Mandatory = mandatory;
        }

        public EditRegistrationPromptViewModel(CustomPrompt customPrompt)
        {
            PromptNumber = customPrompt.CustomPromptNumber;
            Prompt = customPrompt.CustomPromptText;
            Mandatory = customPrompt.Mandatory;
            OptionsString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(customPrompt.Options);
        }

        public int PromptNumber { get; set; }

        public string Prompt { get; set; }

        public bool Mandatory { get; set; }
    }
}
