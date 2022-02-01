namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using DigitalLearningSolutions.Data.Models;

    public class EditCourseSectionViewModel : EditCourseSectionFormData
    {
        public EditCourseSectionViewModel(
            int customisationId,
            string courseName,
            Section section,
            bool showDiagnostic
        ) : base(section, showDiagnostic)
        {
            CourseName = courseName;
            CustomisationId = customisationId;
        }

        public EditCourseSectionViewModel(
            EditCourseSectionFormData formData,
            int customisationId
        ) : base(formData)
        {
            CustomisationId = customisationId;
        }

        public string CourseName { get; set; }

        public int CustomisationId { get; set; }
    }
}
