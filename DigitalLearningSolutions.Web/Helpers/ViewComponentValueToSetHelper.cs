namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static class ViewComponentValueToSetHelper
    {
        public static string? DeriveValueToSet(ref string aspFor, bool populateWithCurrentValue, object model, ViewDataDictionary viewData, out IEnumerable<string> errorMessages)
        {
            string valueToSet;
            var types = aspFor.Split('.');

            if (types.Length == 1)
            {
                valueToSet = ValueToSetForSimpleType(
                    model,
                    aspFor,
                    populateWithCurrentValue,
                    viewData,
                    out errorMessages
                );
            }
            else
            {
                valueToSet = ValueToSetForComplexType(
                    model,
                    aspFor,
                    populateWithCurrentValue,
                    types,
                    viewData,
                    out errorMessages
                );
                aspFor = types[1];
            }

            return valueToSet;
        }

        public static string? ValueToSetForSimpleType(object model, string aspFor, bool populateWithCurrentValue, ViewDataDictionary viewData, out IEnumerable<string> errorMessages)
        {

            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = populateWithCurrentValue ? property?.GetValue(model)?.ToString() : null;

            errorMessages = viewData.ModelState[property?.Name]?.Errors.Select(e => e.ErrorMessage) ??
                            new string[] { };

            return valueToSet;
        }
        public static string? ValueToSetForComplexType(object model, string aspFor, bool populateWithCurrentValue, string[] types, ViewDataDictionary viewData, out IEnumerable<string> errorMessages)
        {
            var firstProperty = model.GetType().GetProperty(types[0]);
            var nestedProperty = firstProperty.PropertyType.GetProperty(types[1]);

            var valueToSetOfFirstProperty = populateWithCurrentValue ? firstProperty?.GetValue(model) : null;
            var valueToSetOfNestedProperty = populateWithCurrentValue ? nestedProperty?.GetValue(valueToSetOfFirstProperty)?.ToString() : null;

            errorMessages = viewData.ModelState[firstProperty?.Name]?.Errors.Select(e => e.ErrorMessage) ??
                            new string[] { };

            return valueToSetOfNestedProperty;
        }
    }
}
