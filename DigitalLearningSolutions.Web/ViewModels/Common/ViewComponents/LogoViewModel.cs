namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using global::DigitalLearningSolutions.Data.Models;

    public class LogoViewModel
    {
        public readonly Logo? Logo;

        public LogoViewModel(Logo? logo)
        {
            Logo = logo;
        }
    }
}
