namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class DelegateRegistrationPrompt : DelegatePrompt
    {
        public DelegateRegistrationPrompt(int promptNumber, string prompt, bool mandatory, string? answer)
        {
            PromptNumber = promptNumber;
            Prompt = prompt;
            Mandatory = mandatory;
            Answer = answer;
        }

        public bool Mandatory { get; set; }
    }
}
