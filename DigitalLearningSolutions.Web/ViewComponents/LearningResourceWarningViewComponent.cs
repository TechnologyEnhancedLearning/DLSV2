namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class LearningResourceWarningViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string text,
            string cssClass
        )
        {
            var model = new InsetTextViewModel(text, string.IsNullOrEmpty(cssClass) ? null : cssClass);
            return View(model);
        }
    }
}
