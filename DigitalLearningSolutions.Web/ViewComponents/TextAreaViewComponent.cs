namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class TextAreaViewComponent : ViewComponent
    {
        /// <summary>
        ///     Render TextArea view component.
        /// </summary>
        /// <param name="aspFor"></param>
        /// <param name="label"></param>
        /// <param name="populateWithCurrentValue"></param>
        /// <param name="rows"></param>
        /// <param name="spellCheck"></param>
        /// <param name="hintText">Leave blank for no hint.</param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            bool populateWithCurrentValue,
            int rows,
            bool spellCheck,
            string hintText,
            string cssClass)
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = populateWithCurrentValue ? property?.GetValue(model)?.ToString() : null;

            var hasError = ViewData.ModelState[property?.Name]?.Errors?.Count > 0;
            var errorMessage = hasError ? ViewData.ModelState[property?.Name]?.Errors[0].ErrorMessage : null;

            var textBoxViewModel = new TextAreaViewModel(
                aspFor,
                aspFor,
                label,
                valueToSet,
                rows,
                spellCheck,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                errorMessage,
                hasError);
            return View(textBoxViewModel);
        }
    }
}
