namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class RegistrationEmailValidator
    {
        private const string WrongEmailForCentreErrorMessage =
            "This email address does not match the one held by the centre; " +
            "either your primary email or centre email must match the one held by the centre";

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
            ICentresDataService centresDataService
        )
        {
            ValidateEmailAddresses(true, model, modelState, userService, centresDataService);
        }

        private static void ValidateEmailAddresses(
            bool isRegisterAdminJourney,
            PersonalInformationViewModel model,
            ModelStateDictionary modelState,
            IUserService userService,
            ICentresDataService? centresDataService = null
        )
        {
            var primaryEmailIsValidAndNotNull =
                !modelState.HasError(nameof(PersonalInformationViewModel.PrimaryEmail)) &&
                model.PrimaryEmail != null;
            var centreSpecificEmailIsValidAndNotNull =
                !modelState.HasError(nameof(PersonalInformationViewModel.CentreSpecificEmail)) &&
                model.CentreSpecificEmail != null;

            var autoRegisterManagerEmail = isRegisterAdminJourney
                ? centresDataService!.GetCentreAutoRegisterValues(model.Centre!.Value)
                    .autoRegisterManagerEmail
                : null;

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
                    if (!StringHelper.StringsMatchCaseInsensitive(model.PrimaryEmail, autoRegisterManagerEmail!) &&
                        model.CentreSpecificEmail == null)
                    {
                        modelState.AddModelError(
                            nameof(PersonalInformationViewModel.PrimaryEmail),
                            WrongEmailForCentreErrorMessage
                        );
                    }
                }
            }

            if (centreSpecificEmailIsValidAndNotNull)
            {
                if (userService.EmailIsInUse(model.CentreSpecificEmail))
                {
                    modelState.AddModelError(
                        nameof(PersonalInformationViewModel.CentreSpecificEmail),
                        DuplicateEmailErrorMessage
                    );
                }
                else if (isRegisterAdminJourney &&
                         !StringHelper.StringsMatchCaseInsensitive(model.PrimaryEmail!, autoRegisterManagerEmail!) &&
                         !StringHelper.StringsMatchCaseInsensitive(model.CentreSpecificEmail, autoRegisterManagerEmail))
                {
                    modelState.AddModelError(
                        nameof(PersonalInformationViewModel.CentreSpecificEmail),
                        WrongEmailForCentreErrorMessage
                    );
                }
            }
        }
    }
}
