namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class ConfirmCheckboxViewComponent : ViewComponent
    {
        /// <summary>
        ///     Render ConfirmCheckboxViewComponent component.
        /// </summary>
        /// <param name="aspFor"></param>
        /// <param name="label"></param>
        /// <param name="hintText">Leave blank for no hint.</param>
        /// <param name="populateWithCurrentValues">Initial current value</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            string? hintText,
            bool populateWithCurrentValues
        )
        {
            var model = ViewData.Model;
            var property = model.GetType().GetProperty(aspFor);
            var hasError = ViewData.ModelState[property?.Name]?.Errors?.Count > 0;
            var errorMessage = hasError ? ViewData.ModelState[property?.Name]?.Errors[0].ErrorMessage : string.Empty;
            var value = (bool)property?.GetValue(model)!;

            var checkboxViewModel = new ConfirmCheckboxViewModel(
                aspFor,
                aspFor,
                label,
                value,
                hintText,
                errorMessage,
                hasError
            );

            return View(checkboxViewModel);
        }
    }
}
