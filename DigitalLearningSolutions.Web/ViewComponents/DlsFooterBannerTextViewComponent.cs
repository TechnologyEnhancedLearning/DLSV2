using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.ViewComponents
{
    public class DlsFooterBannerTextViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string centreName)
        {
            var model = new DlsFooterBannerTextViewModel(centreName);
            return View(model);
        }
    }
}
