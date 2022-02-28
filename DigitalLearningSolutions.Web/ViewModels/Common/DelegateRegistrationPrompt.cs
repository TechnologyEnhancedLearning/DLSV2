namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class DelegateRegistrationPrompt : DelegatePrompt
    {
        public DelegateRegistrationPrompt(int promptNumber, string prompt, bool mandatory, string? answer) : base(
            promptNumber,
            prompt,
            answer
        )
        {
            Mandatory = mandatory;
        }

        public bool Mandatory { get; set; }
    }
}
