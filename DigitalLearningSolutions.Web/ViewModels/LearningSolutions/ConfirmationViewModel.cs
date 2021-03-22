namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    public class ConfirmationViewModel
    {
        public readonly string? Title;
        public readonly string[] Description;

        public ConfirmationViewModel(string? title, string[]? description)
        {
            this.Title = title;
            this.Description = description ?? new string[0];
        }
    }
}
