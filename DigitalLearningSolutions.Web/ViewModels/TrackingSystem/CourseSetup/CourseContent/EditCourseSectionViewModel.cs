namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class EditCourseSectionViewModel
    {
        public EditCourseSectionViewModel() { }

        public EditCourseSectionViewModel(
            int customisationId,
            string courseName,
            Section section
        )
        {
            CustomisationId = customisationId;
            CourseName = courseName;
            SectionId = section.SectionId;
            SectionName = section.SectionName;
            Tutorials = section.Tutorials.Select(t => new EditCourseTutorialViewModel(t));
        }

        public int CustomisationId { get; set; }
        public string CourseName { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }

        public IEnumerable<EditCourseTutorialViewModel> Tutorials { get; set; }
    }
}
