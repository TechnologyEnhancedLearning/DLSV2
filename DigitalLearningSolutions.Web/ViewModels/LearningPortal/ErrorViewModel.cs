namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    public class ErrorViewModel
    {
        private readonly string? bannerText;

        public ErrorViewModel(string? bannerText)
        {
            this.bannerText = bannerText;
        }

        public string HelpText()
        {
            return bannerText == null
                ? "For support, please contact your centre."
                : $"For support, please contact your centre. Contact details: {bannerText}";
        }
    }
}
