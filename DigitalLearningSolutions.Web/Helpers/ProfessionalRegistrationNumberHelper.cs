namespace DigitalLearningSolutions.Web.Helpers
{
    using DocumentFormat.OpenXml.ExtendedProperties;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.CodeAnalysis;
    using System.Diagnostics.Metrics;
    using System.Text.RegularExpressions;

    public class ProfessionalRegistrationNumberHelper
    {
        public static bool? GetHasProfessionalRegistrationNumberForView(bool? hasBeenPromptedForPrn, string? prn)
        {
            if (!hasBeenPromptedForPrn.HasValue || hasBeenPromptedForPrn == false)
            {
                return null;
            }

            return !string.IsNullOrEmpty(prn);
        }

        public static void ValidateProfessionalRegistrationNumber(
            ModelStateDictionary modelState,
            bool? hasPrn,
            string? prn)
        {
            if (hasPrn == false)
            {
                return;
            }

            if (!hasPrn.HasValue)
            {
                modelState.AddModelError(
                    "HasProfessionalRegistrationNumber",
                    "Select a professional registration number status"
                );
                return;
            }

            if (string.IsNullOrEmpty(prn))
            {
                modelState.AddModelError("ProfessionalRegistrationNumber", "Enter a professional registration number");
                return;
            }

            if (prn.Length < 5 || prn.Length > 20)
            {
                modelState.AddModelError(
                    "ProfessionalRegistrationNumber",
                    "Professional registration number must be between 5 and 20 characters"
                );
            }

           const string pattern = @"^(\d{7}|[A-Za-z]{1,2}\d{6}|\d{4,8}|P?\d{5,6}|[C|P]\d{6}|[A-Za-z]?\d{5,6}|L\d{4,6}|\d{2}-[A-Za-z\d]{4,5})$";
            var rg = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!rg.Match(prn).Success)
            {
                modelState.AddModelError(
                    "ProfessionalRegistrationNumber",
                   "Invalid professional registration number format. " +
        "Valid formats include: 7 digits (e.g., 1234567), 1–2 letters followed by 6 digits (e.g., AB123456), " +
        "4–8 digits, an optional 'P' plus 5–6 digits, 'C' or 'P' plus 6 digits, " +
        "an optional letter plus 5–6 digits, 'L' plus 4–6 digits, " +
        "or 2 digits followed by a hyphen and 4–5 alphanumeric characters (e.g., 12-AB123)."
                );
            }
        }
    }
}
