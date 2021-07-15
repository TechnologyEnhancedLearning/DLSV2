namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class SearchBoxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            BaseSearchablePageViewModel searchablePageViewModel,
            string label,
            string? cssClass
        )
        {
            return View(new SearchBoxViewModel(searchablePageViewModel, label, cssClass));
        }
    }
}
