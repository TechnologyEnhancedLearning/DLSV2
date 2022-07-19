namespace DigitalLearningSolutions.Web.Extensions
{
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddAdminField;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditAdminField;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditRegistrationPrompt;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;

    public static class TempDataModelMappingExtensions
    {
        public static CourseDetailsTempData ToCourseDetailsTempData(this SetCourseDetailsViewModel model)
        {
            return new CourseDetailsTempData(
                model.ApplicationId,
                model.ApplicationName,
                model.CustomisationName,
                model.PasswordProtected,
                model.Password,
                model.ReceiveNotificationEmails,
                model.NotificationEmails,
                model.PostLearningAssessment,
                model.IsAssessed,
                model.DiagAssess,
                model.TutCompletionThreshold,
                model.DiagCompletionThreshold
            );
        }

        public static CourseOptionsTempData ToCourseOptionsTempData(this EditCourseOptionsFormData model)
        {
            return new CourseOptionsTempData(
                model.Active,
                model.AllowSelfEnrolment,
                model.DiagnosticObjectiveSelection,
                model.HideInLearningPortal
            );
        }

        public static CourseContentTempData ToDataCourseContentTempData(this SetCourseContentViewModel model)
        {
            return new CourseContentTempData(
                model.AvailableSections,
                model.IncludeAllSections,
                model.SelectedSectionIds
            );
        }

        public static EditRegistrationPromptTempData ToEditRegistrationPromptTempData(this EditRegistrationPromptViewModel model)
        {
            return new EditRegistrationPromptTempData
            {
                PromptNumber = model.PromptNumber,
                Prompt = model.Prompt,
                Mandatory = model.Mandatory,
                OptionsString = model.OptionsString,
                Answer = model.Answer,
                IncludeAnswersTableCaption = model.IncludeAnswersTableCaption,
            };
        }

        public static RegistrationPromptAnswersTempData ToDataConfigureAnswersTempData(this RegistrationPromptAnswersViewModel model)
        {
            return new RegistrationPromptAnswersTempData(
                model.OptionsString,
                model.Answer,
                model.IncludeAnswersTableCaption
            );
        }

        public static EditAdminFieldTempData ToEditAdminFieldTempData(this EditAdminFieldViewModel model)
        {
            return new EditAdminFieldTempData
            {
                PromptNumber = model.PromptNumber,
                Prompt = model.Prompt,
                OptionsString = model.OptionsString,
                Answer = model.Answer,
                IncludeAnswersTableCaption = model.IncludeAnswersTableCaption,
            };
        }

        public static AddAdminFieldTempData ToAddAdminFieldTempData(this AddAdminFieldViewModel model)
        {
            return new AddAdminFieldTempData
            {
                AdminFieldId = model.AdminFieldId,
                OptionsString = model.OptionsString,
                Answer = model.Answer,
                IncludeAnswersTableCaption = model.IncludeAnswersTableCaption,
            };
        }
    }
}
