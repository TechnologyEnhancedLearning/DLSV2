namespace DigitalLearningSolutions.Web.Helpers
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System.ComponentModel.DataAnnotations;

    public static class ValidatableObjectHelper
    {
        public static void ValidateManually(this IValidatableObject model, ModelStateDictionary modelState)
        {
            var validationResults = model.Validate(new ValidationContext(model, null, null));
            foreach (var result in validationResults)
            {
                var key = string.Join("", result.MemberNames);
                if (modelState.TryGetValue(key, out var value))
                {
                    if (value.ValidationState == ModelValidationState.Invalid)
                    {
                        continue;
                    }
                }

                modelState.TryAddModelError(
                    key,
                    result.ErrorMessage
                );
            }
        }
    }
}
