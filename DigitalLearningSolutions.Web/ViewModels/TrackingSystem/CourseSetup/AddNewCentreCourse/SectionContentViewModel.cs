namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;

    public class SectionContentViewModel
    {
        public SectionContentViewModel() {}

        public SectionContentViewModel(SelectSectionViewModel section, bool postLearningAssessment)
        {
            SectionId = section.Id;
            SectionName = section.Name;
            PostLearningAssessment = postLearningAssessment;
        }

        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public IEnumerable<CourseTutorialViewModel> Tutorials { get; set; }
        public bool PostLearningAssessment { get; set; }

        public void SetTutorials(IEnumerable<Tutorial> tutorials)
        {
            Tutorials = tutorials.Select(t => new CourseTutorialViewModel(t));
        }
    }
}
