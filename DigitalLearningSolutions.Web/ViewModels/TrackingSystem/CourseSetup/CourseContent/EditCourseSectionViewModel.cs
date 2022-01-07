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
        ) : base(section, courseName, showDiagnostic)
        {
            CustomisationId = customisationId;
        }

        public EditCourseSectionViewModel(
            EditCourseSectionFormData formData,
            int customisationId
        ) : base(formData)
        {
            CustomisationId = customisationId;
        }

        public int CustomisationId { get; set; }

        
    }
}
