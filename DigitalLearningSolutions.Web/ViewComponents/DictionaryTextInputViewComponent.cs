namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class DictionaryTextInputViewComponent : ViewComponent
    {
        /// <summary>
        ///     Render DictionaryTextInput view component.
        /// </summary>
        /// <param name="aspFor"></param>
        /// <param name="spellCheck"></param>
        /// <param name="hintText">Leave blank for no hint.</param>
        /// <param name="autocomplete">Leave blank to set no autocomplete on the input element.</param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string aspFor,
            bool spellCheck,
            string autocomplete,
            string cssClass,
            string hintText
        )
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(
                aspFor,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly
            );

            var value = property?.GetValue(model);

            var labelsAndValuesById = value != null
                ? (Dictionary<string, (string, string?)>)value
                : new Dictionary<string, (string, string?)>();

            var errorMessages = labelsAndValuesById.Keys.ToDictionary(
                id => id,
                id => ViewData.ModelState[$"{property?.Name}_{id}"]?.Errors.Select(e => e.ErrorMessage) ??
                      new string[] { }
            );

            var dictionaryTextInputViewModel = new DictionaryTextInputViewModel(
                aspFor,
                labelsAndValuesById,
                spellCheck,
                string.IsNullOrEmpty(autocomplete) ? null : autocomplete,
                errorMessages,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText
            );

            return View(dictionaryTextInputViewModel);
        }
    }
}
