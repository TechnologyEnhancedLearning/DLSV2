namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class NumericInputWithConfigurableModelViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            object model,
            string aspFor,
            string label,
            bool populateWithCurrentValue,
            string type,
            string hintText,
            string cssClass
        )
        {
            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = populateWithCurrentValue ? property?.GetValue(model)?.ToString() : null;

            var errorMessages = ViewData.ModelState[property?.Name]?.Errors.Select(e => e.ErrorMessage) ??
                                new string[] { };

            var numericInputViewModel = new NumericInputViewModel(
                aspFor,
                aspFor,
                label,
                valueToSet,
                type,
                errorMessages,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText
            );
            return View("~/Views/Shared/Components/NumericInput/Default.cshtml", numericInputViewModel);
        }
    }
}
