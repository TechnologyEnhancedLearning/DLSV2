namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using DigitalLearningSolutions.Data.Models;

    public class LogoViewModel
    {
        public readonly string? CustomLogoDataUrl;
        public readonly int Height;
        public readonly int Width;
        public readonly string AltText;

        public LogoViewModel(CentreLogo? customLogo)
        {
            CustomLogoDataUrl = customLogo?.LogoUrl;
            Height = customLogo?.Height ?? 0 ;
            Width = customLogo?.Width ?? 0;
            AltText = customLogo?.CentreName ?? "";
        }
    }
}
