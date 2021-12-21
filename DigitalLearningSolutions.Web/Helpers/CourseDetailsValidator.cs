namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
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
            if (string.IsNullOrEmpty(formData.CustomisationName) && courseService.DoesCourseNameExistAtCentre(
                formData.CustomisationName,
                centreId,
                formData.ApplicationId,
                customisationId
            ))
            {
                modelState.ClearErrorsOnField(nameof(EditCourseDetailsViewModel.CustomisationName));
                modelState.AddModelError(
                    nameof(EditCourseDetailsViewModel.CustomisationName),
                    "A course with no add on already exists"
                );
            }
            else if (string.IsNullOrEmpty(formData.CustomisationName))
            {
                modelState.ClearErrorsOnField(nameof(EditCourseDetailsViewModel.CustomisationName));
            }
            else if (formData.CustomisationName.Length > 250)
            {
                modelState.AddModelError(
                    nameof(EditCourseDetailsViewModel.CustomisationName),
                    "Course name must be 250 characters or fewer, including any additions"
                );
            }
            else if (courseService.DoesCourseNameExistAtCentre(
                formData.CustomisationName,
                centreId,
                formData.ApplicationId,
                customisationId
            ))
            {
                modelState.AddModelError(
                    nameof(EditCourseDetailsViewModel.CustomisationName),
                    "Course name must be unique, including any additions"
                );
            }
        }

        public static void ValidatePassword(EditCourseDetailsFormData formData, ModelStateDictionary modelState)
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

        public static void ValidateEmail(EditCourseDetailsFormData formData, ModelStateDictionary modelState)
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

        public static void ValidateCompletionCriteria(
            EditCourseDetailsFormData formData,
            ModelStateDictionary modelState
        )
        {
            if (!formData.IsAssessed)
            {
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
            ;
        }
    }
}
