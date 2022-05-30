namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class RegistrationEmailValidator
    {
        public static void ValidateEmailAddresses(
            PersonalInformationViewModel model,
            ModelStateDictionary modelState,
            IUserService userService,
            ICentresService? centresService = null
        )
        {
            if (model.Email == null || !model.Centre.HasValue)
            {
                return;
            }

            if (userService.EmailIsInUse(model.Email))
            {
                modelState.AddModelError(
                    nameof(PersonalInformationViewModel.Email),
                    "A user with this primary email address is already registered; " +
                    "if this is you, please log in at this centre via the My Account page"
                );
            }
            else if (centresService != null && model.Centre != null && !centresService.DoesEmailMatchCentre(
                model.SecondaryEmail ?? model.Email,
                model.Centre.Value
            ))
            {
                modelState.AddModelError(
                    nameof(PersonalInformationViewModel.Email),
                    "This email address does not match the one held by the centre"
                );
            }

            if (model.SecondaryEmail != null && userService.EmailIsInUse(model.SecondaryEmail))
            {
                modelState.AddModelError(
                    nameof(PersonalInformationViewModel.SecondaryEmail),
                    "A user with this email address is already registered at this centre; " +
                    "if this is you, please log in at this centre via the My Account page"
                );
            }
        }
    }
}
