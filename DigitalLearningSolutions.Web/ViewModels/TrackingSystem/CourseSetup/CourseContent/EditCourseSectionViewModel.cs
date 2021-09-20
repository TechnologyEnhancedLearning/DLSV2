namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class EditCourseSectionViewModel
    {
        public EditCourseSectionViewModel() {}

        public EditCourseSectionViewModel(Section section, bool postLearningAssessment)
        {
            SectionId = section.SectionId;
            SectionName = section.SectionName;
            Tutorials = section.Tutorials.Select(t => new EditCourseTutorialViewModel(t));
            PostLearningAssessment = postLearningAssessment;
        }

        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public IEnumerable<EditCourseTutorialViewModel> Tutorials { get; set; }
        public bool PostLearningAssessment { get; set; }
    }
}
