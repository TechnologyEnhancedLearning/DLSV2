namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class EditCourseSectionFormData
    {
        public EditCourseSectionFormData() { }

        public EditCourseSectionFormData(Section section, string courseName)
        {
            CourseName = courseName;
            SectionName = section.SectionName;
            Tutorials = section.Tutorials.Select(t => new EditCourseTutorialViewModel(t));
        }

        public EditCourseSectionFormData(EditCourseSectionFormData formData)
        {
            CourseName = formData.CourseName;
            SectionName = formData.SectionName;
            Tutorials = formData.Tutorials;
        }

        public string CourseName { get; set; }
        public string SectionName { get; set; }
        public IEnumerable<EditCourseTutorialViewModel> Tutorials { get; set; }
    }
}
