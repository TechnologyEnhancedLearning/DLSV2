using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DigitalLearningSolutions.Web.Helpers
{
    public class RegistrationPasswordValidator
    {
        public static void ValidatePassword(string? password, string forename, string surname, ModelStateDictionary modelState)
        {
            if (password == null || (string)password == string.Empty)
            {
                return;
            }
            if (forename == null || (string)password == string.Empty)
            {
                return;
            }
            if (surname == null || (string)password == string.Empty)
            {
                return;
            }
            string passwordLowercase = password.ToLower();
            string forenameLowercase = forename.ToLower();
            string surnameLowercase = surname.ToLower();

            if (passwordLowercase.Contains(forenameLowercase) || passwordLowercase.Contains(surnameLowercase))
            {
                modelState.AddModelError("Password", CommonValidationErrorMessages.PasswordSimilarUsername);
            }

        }
    }
}
