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

        private const string DuplicateEmailErrorMessage =
            "A user with this email address is already registered; if this is you, " +
            "please log in and register at this centre via the My Account page";

        public static void ValidateEmailAddressesForDelegateRegistration(
            PersonalInformationViewModel model,
            ModelStateDictionary modelState,
            IUserService userService
        )
        {
            ValidateEmailAddresses(false, model, modelState, userService);
        }

        public static void ValidateEmailAddressesForAdminRegistration(
            PersonalInformationViewModel model,
            ModelStateDictionary modelState,
            IUserService userService,
            ICentresService centresService
        )
        {
            ValidateEmailAddresses(true, model, modelState, userService, centresService);
        }

        private static void ValidateEmailAddresses(
            bool isRegisterAdminJourney,
            PersonalInformationViewModel model,
            ModelStateDictionary modelState,
            IUserService userService,
            ICentresService? centresService = null
        )
        {
            var primaryEmailIsValidAndNotNull =
                !modelState.HasError(nameof(PersonalInformationViewModel.PrimaryEmail)) &&
                model.PrimaryEmail != null;
            var secondaryEmailIsValidAndNotNull =
                !modelState.HasError(nameof(PersonalInformationViewModel.SecondaryEmail)) &&
                model.SecondaryEmail != null;

            bool? primaryEmailMatchesCentre = null;

            if (primaryEmailIsValidAndNotNull)
            {
                if (userService.EmailIsInUse(model.PrimaryEmail))
                {
                    modelState.AddModelError(
                        nameof(PersonalInformationViewModel.PrimaryEmail),
                        DuplicateEmailErrorMessage
                    );
                }
                else if (isRegisterAdminJourney)
                {
                    primaryEmailMatchesCentre = centresService?.DoesEmailMatchCentre(
                        model.PrimaryEmail!,
                        model.Centre!.Value
                    );

                    if (primaryEmailMatchesCentre == false && model.SecondaryEmail == null)
                    {
                        modelState.AddModelError(
                            nameof(PersonalInformationViewModel.PrimaryEmail),
                            WrongEmailForCentreErrorMessage
                        );
                    }
                }
            }

            if (secondaryEmailIsValidAndNotNull)
            {
                if (userService.EmailIsInUse(model.SecondaryEmail))
                {
                    modelState.AddModelError(
                        nameof(PersonalInformationViewModel.SecondaryEmail),
                        DuplicateEmailErrorMessage
                    );
                }
                else if (isRegisterAdminJourney && primaryEmailMatchesCentre == false)
                {
                    var secondaryEmailMatchesCentre = centresService?.DoesEmailMatchCentre(
                        model.SecondaryEmail!,
                        model.Centre!.Value
                    );

                    if (secondaryEmailMatchesCentre == false)
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
}
