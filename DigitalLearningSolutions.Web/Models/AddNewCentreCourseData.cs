namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;

    public class AddNewCentreCourseData
    {
        public AddNewCentreCourseData()
        {
            Id = Guid.NewGuid();
            SelectCourseViewModel = new SelectCourseViewModel();
            SetCourseDetailsViewModel = new EditCourseDetailsFormData();
            SetCourseOptionsViewModel = new EditCourseOptionsFormData();
            SetCourseContentViewModel = new SetCourseContentViewModel();
            SetSectionContentViewModel = new SetSectionContentViewModel();
            SummaryViewModel = new SummaryViewModel();
        }

        public Guid Id { get; set; }
        public SelectCourseViewModel SelectCourseViewModel { get; set; }
        public EditCourseDetailsFormData SetCourseDetailsViewModel { get; set; }
        public EditCourseOptionsFormData SetCourseOptionsViewModel { get; set; }
        public SetCourseContentViewModel SetCourseContentViewModel { get; set; }
        public SetSectionContentViewModel SetSectionContentViewModel { get; set; }
        public SummaryViewModel SummaryViewModel { get; set; }

        public void SetCourse(ApplicationDetails application)
        {
            SelectCourseViewModel.Application = application;
        }

        public void SetCourseDetails(EditCourseDetailsFormData formData)
        {
            SetCourseDetailsViewModel.ApplicationId = formData.ApplicationId;
            SetCourseDetailsViewModel.ApplicationName = formData.ApplicationName;
            SetCourseDetailsViewModel.CustomisationName = formData.CustomisationName;
            SetCourseDetailsViewModel.PasswordProtected = formData.PasswordProtected;
            SetCourseDetailsViewModel.Password = formData.Password;
            SetCourseDetailsViewModel.ReceiveNotificationEmails = formData.ReceiveNotificationEmails;
            SetCourseDetailsViewModel.NotificationEmails = formData.NotificationEmails;
            SetCourseDetailsViewModel.PostLearningAssessment = formData.PostLearningAssessment;
            SetCourseDetailsViewModel.IsAssessed = formData.IsAssessed;
            SetCourseDetailsViewModel.DiagAssess = formData.DiagAssess;
            SetCourseDetailsViewModel.TutCompletionThreshold = formData.TutCompletionThreshold;
            SetCourseDetailsViewModel.DiagCompletionThreshold = formData.DiagCompletionThreshold;
        }

        public void SetCourseOptions(EditCourseOptionsFormData model)
        {
            SetCourseOptionsViewModel.Active = model.Active;
            SetCourseOptionsViewModel.AllowSelfEnrolment = model.AllowSelfEnrolment;
            SetCourseOptionsViewModel.HideInLearningPortal = model.HideInLearningPortal;
            SetCourseOptionsViewModel.DiagnosticObjectiveSelection = model.DiagnosticObjectiveSelection;
        }

        public void SetCourseContent(SetCourseContentViewModel model)
        {
            SetCourseContentViewModel.AvailableSections = model.AvailableSections;
            SetCourseContentViewModel.IncludeAllSections = model.IncludeAllSections;
            SetCourseContentViewModel.SectionsToInclude = model.SectionsToInclude;
        }

        public void SetSectionContent(SetSectionContentViewModel model)
        {
            SetSectionContentViewModel.Sections = model.Sections;
        }
    }
}
