namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    public class CustomFieldViewModel
    {
        public CustomFieldViewModel(string prompt, bool mandatory, string? answer)
        {
            CustomPrompt = prompt;
            Mandatory = mandatory;
            Answer = answer;
        }

        public string CustomPrompt { get; set; }

        public bool Mandatory { get; set; }

        public string? Answer { get; set; }
    }
}
