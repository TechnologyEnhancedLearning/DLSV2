namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class TextInputViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            string type,
            bool spellCheck,
            string? hintText,
            string? autocomplete,
            string? cssClass
        )
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(aspFor);
            var propertyValue = property?.GetValue(model)?.ToString();

            var hasError = ViewData.ModelState[property?.Name]?.Errors?.Count > 0;
            var errorMessage = hasError ? ViewData.ModelState[property?.Name]?.Errors[0].ErrorMessage : null;

            var textBoxViewModel = new TextInputViewModel(
                aspFor,
                aspFor,
                label,
                propertyValue,
                type,
                spellCheck,
                autocomplete,
                cssClass,
                hintText,
                errorMessage,
                hasError);
            return View(textBoxViewModel);
        }
    }
}
