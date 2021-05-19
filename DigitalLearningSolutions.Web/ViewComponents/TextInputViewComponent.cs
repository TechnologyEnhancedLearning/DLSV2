﻿namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class TextInputViewComponent : ViewComponent
    {
        /// <summary>
        /// Render TextInput view component.
        /// </summary>
        /// <param name="aspFor"></param>
        /// <param name="label"></param>
        /// <param name="populateWithCurrentValue"></param>
        /// <param name="type"></param>
        /// <param name="spellCheck"></param>
        /// <param name="hintText">Leave blank for no hint.</param>
        /// <param name="autocomplete">Leave blank to set no autocomplete on the input element.</param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            bool populateWithCurrentValue,
            string type,
            bool spellCheck,
            string hintText,
            string autocomplete,
            string cssClass)
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = populateWithCurrentValue ? property?.GetValue(model)?.ToString() : null;

            var hasError = ViewData.ModelState[property?.Name]?.Errors?.Count > 0;
            var errorMessage = hasError ? ViewData.ModelState[property?.Name]?.Errors[0].ErrorMessage : null;

            var textBoxViewModel = new TextInputViewModel(
                aspFor,
                aspFor,
                label,
                valueToSet,
                type,
                spellCheck,
                string.IsNullOrEmpty(autocomplete) ? null : autocomplete,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                errorMessage,
                hasError);
            return View(textBoxViewModel);
        }
    }
}
