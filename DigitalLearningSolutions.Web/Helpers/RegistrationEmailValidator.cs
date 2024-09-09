namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System;

    public static class RegistrationEmailValidator
    {
        public static void ValidatePrimaryEmailIfNecessary(
            string? primaryEmail,
            string nameOfFieldToValidate,
            ModelStateDictionary modelState,
            IUserService userService,
            string errorMessage
        )
        {
            if (
                IsValidationNecessary(primaryEmail, nameOfFieldToValidate, modelState) &&
                (userService.PrimaryEmailIsInUse(primaryEmail!) || userService.PrimaryEmailInUseAtCentres(primaryEmail!))
            )
            {
                modelState.AddModelError(nameOfFieldToValidate, errorMessage);
            }
        }

        public static void ValidateCentreEmailIfNecessary(
            string? centreEmail,
            int? centreId,
            string nameOfFieldToValidate,
            ModelStateDictionary modelState,
            IUserService userService
        )
        {
            if (
                centreId.HasValue &&
                IsValidationNecessary(centreEmail, nameOfFieldToValidate, modelState)
                )
                {
                    if (userService.CentreSpecificEmailIsInUseAtCentre(centreEmail!, centreId.Value))
                    {
                        modelState.AddModelError(nameOfFieldToValidate, CommonValidationErrorMessages.EmailInUseAtCentre);
                    }
                    else if (userService.PrimaryEmailIsInUse(centreEmail!))
                    {
                        modelState.AddModelError(nameOfFieldToValidate, CommonValidationErrorMessages.PrimaryEmailInUseDuringDelegateRegistration);
                    }
                }

        }

        public static void ValidateEmailNotHeldAtCentreIfEmailNotYetValidated(
            string? email,
            int centreId,
            string nameOfFieldToValidate,
            ModelStateDictionary modelState,
            IUserService userService
        )
        {
            if (!IsValidationNecessary(email, nameOfFieldToValidate, modelState))
            {
                return;
            }
            var emailIsHeldAtCentre = userService.EmailIsHeldAtCentre(email, centreId);
            if (emailIsHeldAtCentre)
            {
                modelState.AddModelError(nameOfFieldToValidate, CommonValidationErrorMessages.EmailInUseAtCentre);
            }
        }

        public static void ValidateCentreEmailWithUserIdIfNecessary(
            string? centreEmail,
            int? centreId,
            int userId,
            string nameOfFieldToValidate,
            ModelStateDictionary modelState,
            IUserService userService
        )
        {
            if (
                centreId.HasValue &&
                IsValidationNecessary(centreEmail, nameOfFieldToValidate, modelState) &&
                userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(centreEmail!, centreId.Value, userId)
            )
            {
                modelState.AddModelError(nameOfFieldToValidate, CommonValidationErrorMessages.EmailInUseAtCentre);
            }
        }

        public static void ValidateEmailsForCentreManagerIfNecessary(
            string? primaryEmail,
            string? centreEmail,
            int? centreId,
            string nameOfFieldToValidate,
            ModelStateDictionary modelState,
            ICentresService centresService
        )
        {
            if (
                modelState.IsValid && centreId.HasValue && (primaryEmail != null || centreEmail != null) &&
                !centresService.IsAnEmailValidForCentreManager(primaryEmail, centreEmail, centreId.Value)
            )
            {
                modelState.AddModelError(
                    nameOfFieldToValidate,
                    CommonValidationErrorMessages.WrongEmailForCentreDuringAdminRegistration
                );
            }
        }

        private static bool IsValidationNecessary(
            string? email,
            string nameOfFieldToValidate,
            ModelStateDictionary modelState
        )
        {
            return email != null && !modelState.HasError(nameOfFieldToValidate);
        }
    }
}
