namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditRegistrationPrompt;

    public class EditRegistrationPromptViewModel : RegistrationPromptAnswersViewModel
    {
        public EditRegistrationPromptViewModel()
        {
            IncludeAnswersTableCaption = true;
        }

        public EditRegistrationPromptViewModel(
            int promptNumber,
            string prompt,
            bool mandatory,
            string optionsString,
            string? answer = null
        )
            : base(optionsString, answer, true)
        {
            PromptNumber = promptNumber;
            Prompt = prompt;
            Mandatory = mandatory;
        }

        public EditRegistrationPromptViewModel(CentreRegistrationPrompt centreRegistrationPrompt)
        {
            PromptNumber = centreRegistrationPrompt.RegistrationField.Id;
            Prompt = centreRegistrationPrompt.PromptText;
            Mandatory = centreRegistrationPrompt.Mandatory;
            OptionsString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(centreRegistrationPrompt.Options);
            IncludeAnswersTableCaption = true;
        }

        public EditRegistrationPromptViewModel(EditRegistrationPromptTempData tempData) : base(
            tempData.OptionsString,
            tempData.Answer,
            tempData.IncludeAnswersTableCaption
        )
        {
            Prompt = tempData.Prompt;
            PromptNumber = tempData.PromptNumber;
            Mandatory = tempData.Mandatory;
        }

        public int PromptNumber { get; set; }

        public string Prompt { get; set; }

        public bool Mandatory { get; set; }
    }
}
