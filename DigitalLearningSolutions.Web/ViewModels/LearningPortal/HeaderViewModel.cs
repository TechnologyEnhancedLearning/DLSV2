namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using DigitalLearningSolutions.Data.Models;

    public class HeaderViewModel
    {
        public readonly string CustomLogoDataUrl;
        public readonly int Height;
        public readonly int Width;

        public HeaderViewModel(CentreLogo customLogo)
        {
            CustomLogoDataUrl = customLogo.LogoUrl;
            Height = customLogo.Height;
            Width = customLogo.Width;
        }
    }
}
