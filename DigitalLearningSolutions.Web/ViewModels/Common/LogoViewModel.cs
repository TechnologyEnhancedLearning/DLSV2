namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using DigitalLearningSolutions.Data.Models;

    public class LogoViewModel
    {
        public readonly Logo? Logo;

        public LogoViewModel(Logo? logo)
        {
            Logo = logo;
        }
    }
}
