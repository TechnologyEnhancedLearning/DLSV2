namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class SearchBoxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspController,
            string aspAction,
            BaseSearchablePageViewModel searchablePageViewModel,
            string label,
            string? cssClass
        )
        {
            return View(new SearchBoxViewModel(aspController, aspAction, searchablePageViewModel, label, cssClass));
        }
    }
}
