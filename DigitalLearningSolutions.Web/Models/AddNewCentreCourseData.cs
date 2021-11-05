namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;

    public class AddNewCentreCourseData
    {
        public AddNewCentreCourseData()
        {
            Id = Guid.NewGuid();
            SelectCourseViewModel = new SelectCourseViewModel();
        }

        public Guid Id { get; set; }

        public SelectCourseViewModel SelectCourseViewModel { get; set; }
        // TODO: public EditCourseDetailsViewModel EditCourseDetailsViewModel { get; set; }
        public EditCourseOptionsViewModel SetCourseOptionsViewModel { get; set; }
        public CourseContentViewModel CourseContentViewModel { get; set; }
        public EditCourseSectionViewModel EditCourseSectionViewModel { get; set; }

        public void SetCourse(SelectCourseViewModel model)
        {
            SelectCourseViewModel.CustomisationId = model.CustomisationId;
        }

        public void SetCourseDetails(EditCourseDetailsViewModel model)
        {
            // TODO: Set course details
        }

        public void SetCourseOptions(EditCourseOptionsViewModel model)
        {
            SetCourseOptionsViewModel.Active = model.Active;
            SetCourseOptionsViewModel.AllowSelfEnrolment = model.AllowSelfEnrolment;
            SetCourseOptionsViewModel.HideInLearningPortal = model.HideInLearningPortal,
            SetCourseOptionsViewModel.DiagnosticObjectiveSelection = model.DiagnosticObjectiveSelection,
        }

        public void SetCourseContent(EditCourseSectionViewModel model)
        {
            // TODO: Set course content
        }

        public void EditSectionContent(EditCourseSectionViewModel model)
        {
            // TODO: Set section content
        }
    }
}
