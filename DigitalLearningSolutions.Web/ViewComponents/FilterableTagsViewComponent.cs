namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class FilterableTagsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<SearchableTagViewModel> tags)
        {
            return View(tags);
        }
    }
}
