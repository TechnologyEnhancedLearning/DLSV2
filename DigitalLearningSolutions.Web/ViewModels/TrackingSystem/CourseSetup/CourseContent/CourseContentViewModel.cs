namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class CourseContentViewModel
    {
        public CourseContentViewModel(
            int customisationId,
            string courseName,
            bool postLearningAssessment,
            IEnumerable<Section> sections
        )
        {
            CustomisationId = customisationId;
            CourseName = courseName;
            Sections = sections.Select(s => new CourseSectionViewModel(s, customisationId, postLearningAssessment));
        }

        public int CustomisationId { get; set; }
        public string CourseName { get; set; }
        public IEnumerable<CourseSectionViewModel> Sections { get; set; }

        public bool CourseHasContent => Sections.Any(
            s => s.Tutorials.Any(t => t.DiagnosticEnabled || t.LearningEnabled) || s.PostLearningAssessment
        );
    }
}
