namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class EditCourseSectionFormData
    {
        public EditCourseSectionFormData() { }

        public EditCourseSectionFormData(Section section, string courseName, bool showDiagnostic)
        {
            CourseName = courseName;
            SectionName = section.SectionName;
            ShowDiagnostic = showDiagnostic;
            Tutorials = section.Tutorials.Select(t => new CourseTutorialViewModel(t));
        }

        protected EditCourseSectionFormData(EditCourseSectionFormData formData)
        {
            CourseName = formData.CourseName;
            SectionName = formData.SectionName;
            ShowDiagnostic = formData.ShowDiagnostic;
            Tutorials = formData.Tutorials;
        }

        public string CourseName { get; set; }
        public string SectionName { get; set; }
        public bool ShowDiagnostic { get; set; }
        public IEnumerable<CourseTutorialViewModel> Tutorials { get; set; }
    }
}
