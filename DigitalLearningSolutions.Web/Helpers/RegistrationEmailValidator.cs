namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class RegistrationEmailValidator
    {
        private const string WrongEmailForCentreErrorMessage =
            "This email address does not match the one held by the centre";

        private const string EmailInUseErrorMessage =
            "A user with this email address is already registered; if this is you, " +
            "please log in at this centre via the My Account page";

        public static void ValidateEmailAddresses(
            PersonalInformationViewModel model,
            ModelStateDictionary modelState,
            IUserService userService,
            ICentresService? centresService = null
        )
        {
            if (!model.Centre.HasValue)
            {
                return;
            }

            var isRegisterAdminJourney = centresService != null;
            var primaryEmailIsValid = !modelState.HasError(nameof(PersonalInformationViewModel.PrimaryEmail));
            var secondaryEmailIsValid = !modelState.HasError(nameof(PersonalInformationViewModel.SecondaryEmail));

            if (primaryEmailIsValid && model.PrimaryEmail != null)
            {
                if (userService.EmailIsInUse(model.PrimaryEmail!))
                {
                    modelState.AddModelError(
                        nameof(PersonalInformationViewModel.PrimaryEmail),
                        EmailInUseErrorMessage
                    );
                }
                else if (isRegisterAdminJourney && model.SecondaryEmail == null && !centresService.DoesEmailMatchCentre(
                    model.PrimaryEmail,
                    model.Centre.Value
                ))
                {
                    modelState.AddModelError(
                        nameof(PersonalInformationViewModel.PrimaryEmail),
                        WrongEmailForCentreErrorMessage
                    );
                }
            }

            if (secondaryEmailIsValid)
            {
                if (model.SecondaryEmail != null && userService.EmailIsInUse(model.SecondaryEmail))
                {
                    modelState.AddModelError(
                        nameof(PersonalInformationViewModel.SecondaryEmail),
                        EmailInUseErrorMessage
                    );
                }
                else if (isRegisterAdminJourney && model.SecondaryEmail != null && !centresService.DoesEmailMatchCentre(
                    model.SecondaryEmail,
                    model.Centre.Value
                ))
                {
                    modelState.AddModelError(
                        nameof(PersonalInformationViewModel.SecondaryEmail),
                        WrongEmailForCentreErrorMessage
                    );
                }
            }
        }
    }
}
