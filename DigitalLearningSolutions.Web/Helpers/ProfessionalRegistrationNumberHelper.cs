namespace DigitalLearningSolutions.Web.Helpers
{

    using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            const string pattern = @"^(\d{7}|[A-Za-z]{1,2}\d{6}|\d{4,8}|P?\d{5,6}|[C|P]\d{6}|[A-Za-z]?\d{5,6}|L\d{4,6}|\d{2}-[A-Za-z\d]{4,5})$";
            var rg = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!rg.Match(prn).Success)
            {
                modelState.AddModelError(
                    "ProfessionalRegistrationNumber",
                 GetProfessionalRegistrationNumberErrorMessage()
                );
            }
        }
        public static string GetProfessionalRegistrationNumberErrorMessage()
        {
            return @"The format you entered isn’t recognised. Please check and try again.
            Valid formats include
            7 digits - example, 1234567
            1-2 letters followed by 6 digits - example, AB123456
            ‘P’ followed by 5-6 digits - example, P12345, P123456
            ‘C’ or ‘P’ followed by 6 digits - example, C123456, P123456
            Optional letter followed by 5-6 digits - example, A12345, B123456
            ‘L’ followed by 4-6 digits - example, L1234, L123456
            2 digits, hyphen, then 4-5 alphanumeric characters - example, 12-AB123";
        }
    }
}
