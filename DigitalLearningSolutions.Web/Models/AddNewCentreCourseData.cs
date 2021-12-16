namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
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
            SetCourseSectionViewModel = new EditCourseSectionFormData();
            SummaryViewModel = new SummaryViewModel();
        }

        public Guid Id { get; set; }
        public SelectCourseViewModel SelectCourseViewModel { get; set; }
        public EditCourseDetailsFormData SetCourseDetailsViewModel { get; set; }
        public EditCourseOptionsFormData SetCourseOptionsViewModel { get; set; }
        public SetCourseContentViewModel SetCourseContentViewModel { get; set; }
        public EditCourseSectionFormData SetCourseSectionViewModel { get; set; }
        public SummaryViewModel SummaryViewModel { get; set; }

        public void SetCourse(SelectCourseViewModel model)
        {
            SelectCourseViewModel.CustomisationId = model.CustomisationId;
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
            // TODO: Set course content
        }

        public void SetSectionContent(EditCourseSectionFormData formData)
        {
            SetCourseSectionViewModel.CourseName = formData.CourseName;
            SetCourseSectionViewModel.SectionName = formData.SectionName;
            SetCourseSectionViewModel.Tutorials = formData.Tutorials;
        }

        public void PopulateSummaryData(SummaryViewModel model)
        {
            // TODO: Populate summary data
        }
    }
}
