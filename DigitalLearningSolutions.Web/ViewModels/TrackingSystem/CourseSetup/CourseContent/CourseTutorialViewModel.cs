namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent
{
    using DigitalLearningSolutions.Data.Models;

    public class CourseTutorialViewModel
    {
        public CourseTutorialViewModel() { }

        public CourseTutorialViewModel(Tutorial tutorial)
        {
            TutorialId = tutorial.TutorialId;
            TutorialName = tutorial.TutorialName;
            LearningEnabled = tutorial.Status ?? false;
            DiagnosticEnabled = tutorial.DiagStatus ?? false;
        }

        public int TutorialId { get; set; }
        public string TutorialName { get; set; }
        public bool LearningEnabled { get; set; }
        public bool DiagnosticEnabled { get; set; }
    }
}
