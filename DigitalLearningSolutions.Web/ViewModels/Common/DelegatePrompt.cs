namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public abstract class DelegatePrompt
    {
        protected DelegatePrompt(int promptNumber, string prompt, string? answer)
        {
            PromptNumber = promptNumber;
            Prompt = prompt;
            Answer = answer;
        }

        public int PromptNumber { get; set; }

        public string Prompt { get; set; }

        public string? Answer { get; set; }
    }
}
