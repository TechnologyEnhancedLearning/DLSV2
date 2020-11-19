namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
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
