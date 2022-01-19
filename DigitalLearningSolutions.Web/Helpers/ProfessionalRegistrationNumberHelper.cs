namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            string? prn, 
            bool isDelegateUser = true)
        {
            if (!isDelegateUser || hasPrn == false)
            {
                return;
            }

            if (!hasPrn.HasValue)
            {
                modelState.AddModelError(
                    "HasProfessionalRegistrationNumber",
                    "Select your professional registration number status."
                );
                return;
            }

            if (string.IsNullOrEmpty(prn))
            {
                modelState.AddModelError("ProfessionalRegistrationNumber", "Enter professional registration number.");
                return;
            }

            if (prn.Length < 5 || prn.Length > 20)
            {
                modelState.AddModelError(
                    "ProfessionalRegistrationNumber",
                    "Professional registration number must be between 5 and 20 characters."
                );
            }

            const string pattern = @"^[a-z\d-]+$";
            var rg = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!rg.Match(prn).Success)
            {
                modelState.AddModelError(
                    "ProfessionalRegistrationNumber",
                    "Invalid professional registration number format. Only alphanumeric (a-z, A-Z and 0-9) and hyphens (-) allowed."
                );
            }
        }
    }
}
