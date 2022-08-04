namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class RegistrationEmailValidator
    {
        public static void ValidatePrimaryEmailIfNecessary(
            string? primaryEmail,
            string nameOfFieldToValidate,
            ModelStateDictionary modelState,
            IUserDataService userDataService,
            string errorMessage
        )
        {
            if (
                IsValidationNecessary(primaryEmail, nameOfFieldToValidate, modelState) &&
                userDataService.PrimaryEmailIsInUse(primaryEmail!)
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
            IUserDataService userDataService
        )
        {
            if (
                centreId.HasValue &&
                IsValidationNecessary(centreEmail, nameOfFieldToValidate, modelState) &&
                userDataService.CentreSpecificEmailIsInUseAtCentre(centreEmail!, centreId.Value)
            )
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
            IUserDataService userDataService
        )
        {
            if (
                centreId.HasValue &&
                IsValidationNecessary(centreEmail, nameOfFieldToValidate, modelState) &&
                userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(centreEmail!, centreId.Value, userId)
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
