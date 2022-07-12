namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class CourseDetailsValidator
    {
        public static void ValidateCustomisationName(
            EditCourseDetailsFormData formData,
            ModelStateDictionary modelState,
            ICourseService courseService,
            int centreId,
            int customisationId = 0
        )
        {
            formData.TrimCustomisationNameOrConvertToEmptyStringIfNull();

            var courseExistsAtCentre = courseService.DoesCourseNameExistAtCentre(
                formData.CustomisationName!,
                centreId,
                formData.ApplicationId,
                customisationId
            );

            if (string.IsNullOrEmpty(formData.CustomisationName) && courseExistsAtCentre)
            {
                modelState.AddModelError(
                    nameof(formData.CustomisationName),
                    "A course with no add-on already exists"
                );
            }
            else if (string.IsNullOrEmpty(formData.CustomisationName))
            {
                modelState.ClearErrorsOnField(nameof(formData.CustomisationName));
            }
            else if (formData.CustomisationName.Length > 250)
            {
                modelState.AddModelError(
                    nameof(formData.CustomisationName),
                    "Course name must be 250 characters or fewer, including any additions"
                );
            }
            else if (courseExistsAtCentre)
            {
                modelState.AddModelError(
                    nameof(formData.CustomisationName),
                    "Course name must be unique, including any additions"
                );
            }
        }

        public static void ResetValueAndClearErrorsOnPasswordIfUnselected(
            EditCourseDetailsFormData formData,
            ModelStateDictionary modelState
        )
        {
            if (formData.PasswordProtected)
            {
                return;
            }

            if (modelState.HasError(nameof(formData.Password)))
            {
                modelState.ClearErrorsOnField(nameof(formData.Password));
            }

            formData.Password = null;
        }

        public static void ResetValueAndClearErrorsOnEmailIfUnselected(
            EditCourseDetailsFormData formData,
            ModelStateDictionary modelState
        )
        {
            if (formData.ReceiveNotificationEmails)
            {
                return;
            }

            if (modelState.HasError(nameof(formData.NotificationEmails)))
            {
                modelState.ClearErrorsOnField(nameof(formData.NotificationEmails));
            }

            formData.NotificationEmails = null;
        }

        public static void ResetValueAndClearErrorsOnOtherCompletionCriteriaIfUnselected(
            EditCourseDetailsFormData formData,
            ModelStateDictionary modelState
        )
        {
            if (!formData.IsAssessed)
            {
                formData.TutCompletionThreshold ??= "0";
                formData.DiagCompletionThreshold ??= "0";
                return;
            }

            if (modelState.HasError(nameof(formData.TutCompletionThreshold)))
            {
                modelState.ClearErrorsOnField(nameof(formData.TutCompletionThreshold));
            }

            if (modelState.HasError(nameof(formData.DiagCompletionThreshold)))
            {
                modelState.ClearErrorsOnField(nameof(formData.DiagCompletionThreshold));
            }

            formData.TutCompletionThreshold = "0";
            formData.DiagCompletionThreshold = "0";
        }
    }
}
