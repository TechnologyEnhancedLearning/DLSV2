namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class SearchBoxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspController,
            string aspAction,
            BaseSearchablePageViewModel containingPageModel,
            string label
        )
        {
            return View(new SearchBoxViewModel(aspController, aspAction, containingPageModel, label));
        }
    }
}
