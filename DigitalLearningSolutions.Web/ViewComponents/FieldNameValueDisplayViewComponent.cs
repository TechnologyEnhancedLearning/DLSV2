namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class FieldNameValueDisplayViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string displayName, string fieldValue)
        {
            return View(new FieldNameValueDisplayViewModel(displayName, fieldValue));
        }
    }
}
