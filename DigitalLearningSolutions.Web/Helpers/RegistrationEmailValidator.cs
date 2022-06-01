namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
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
            if (model.PrimaryEmail == null || !model.Centre.HasValue)
            {
                return;
            }

            var isRegisterAdminJourney = centresService != null;
            var primaryEmailIsValid = !modelState.HasError(nameof(PersonalInformationViewModel.PrimaryEmail));
            var secondaryEmailIsValid = !modelState.HasError(nameof(PersonalInformationViewModel.SecondaryEmail));

            if (primaryEmailIsValid && userService.EmailIsInUse(model.PrimaryEmail))
            {
                modelState.AddModelError(
                    nameof(PersonalInformationViewModel.PrimaryEmail),
                    "A user with this primary email address is already registered; " +
                    "if this is you, please log in at this centre via the My Account page"
                );
            }
            else if (primaryEmailIsValid && isRegisterAdminJourney &&
                     model.SecondaryEmail == null && !centresService.DoesEmailMatchCentre(
                         model.PrimaryEmail,
                         model.Centre.Value
                     ))
            {
                modelState.AddModelError(
                    nameof(PersonalInformationViewModel.PrimaryEmail),
                    "This email address does not match the one held by the centre"
                );
            }

            if (!secondaryEmailIsValid)
            {
                return;
            }

            if (model.SecondaryEmail != null && userService.EmailIsInUse(model.SecondaryEmail))
            {
                modelState.AddModelError(
                    nameof(PersonalInformationViewModel.SecondaryEmail),
                    "A user with this email address is already registered at this centre; " +
                    "if this is you, please log in at this centre via the My Account page"
                );
            }
            else if (isRegisterAdminJourney && model.SecondaryEmail != null && !centresService.DoesEmailMatchCentre(
                model.SecondaryEmail,
                model.Centre.Value
            ))
            {
                modelState.AddModelError(
                    nameof(PersonalInformationViewModel.SecondaryEmail),
                    "This email address does not match the one held by the centre"
                );
            }
        }
    }
}
