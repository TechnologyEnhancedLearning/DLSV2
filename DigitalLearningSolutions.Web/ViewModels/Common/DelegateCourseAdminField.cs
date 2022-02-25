namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class DelegateCourseAdminField : DelegatePrompt
    {
        public DelegateCourseAdminField(int promptNumber, string prompt, string? answer)
        {
            PromptNumber = promptNumber;
            Prompt = prompt;
            Answer = answer;
        }
    }
}
