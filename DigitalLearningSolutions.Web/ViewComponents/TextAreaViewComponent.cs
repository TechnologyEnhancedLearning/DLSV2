namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
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
        /// <param name="characterCount"></param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            bool populateWithCurrentValue,
            int rows,
            bool spellCheck,
            string hintText,
            string cssClass,
            int? characterCount)
        {
            var model = ViewData.Model;

            string valueToSet = string.Empty;
            string[] types = aspFor.Split('.');
            IEnumerable<string> errorMessages;

            if (types.Length == 1) valueToSet = ValueToSetForSimpleType(model, aspFor, populateWithCurrentValue, out errorMessages);
            else valueToSet = ValueToSetForComplexType(model, aspFor, populateWithCurrentValue, types, out errorMessages);

            var textBoxViewModel = new TextAreaViewModel(
                aspFor,
                aspFor,
                label,
                valueToSet,
                rows,
                spellCheck,
                errorMessages,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                characterCount);
            return View(textBoxViewModel);
        }

        private string ValueToSetForSimpleType(object model, string aspFor, bool populateWithCurrentValue, out IEnumerable<string> errorMessages)
        {

            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = populateWithCurrentValue ? property?.GetValue(model)?.ToString() : null;

            errorMessages = ViewData.ModelState[property?.Name]?.Errors.Select(e => e.ErrorMessage) ??
                                new string[] { };

            return valueToSet;

        }

        private string ValueToSetForComplexType(object model, string aspFor, bool populateWithCurrentValue, string[] types, out IEnumerable<string> errorMessages)
        {
            var firstProperty = model.GetType().GetProperty(types[0]);
            var nestedProperty = firstProperty.PropertyType.GetProperty(types[1]);

            var valueToSetOfFirstProperty = populateWithCurrentValue ? firstProperty?.GetValue(model) : null;
            var valueToSetOfNestedProperty = populateWithCurrentValue ? nestedProperty?.GetValue(valueToSetOfFirstProperty)?.ToString() : null;

            errorMessages = ViewData.ModelState[firstProperty?.Name]?.Errors.Select(e => e.ErrorMessage) ??
                                new string[] { };

            return valueToSetOfNestedProperty;
        }
    }
}
