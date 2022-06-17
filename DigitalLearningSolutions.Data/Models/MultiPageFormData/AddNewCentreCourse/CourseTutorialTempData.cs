namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    public class CourseTutorialTempData
    {
        // 'Unused' constructor required for JsonConvert
        public CourseTutorialTempData(){}

        public CourseTutorialTempData(Tutorial tutorial)
        {
            TutorialId = tutorial.TutorialId;
            TutorialName = tutorial.TutorialName;
            LearningEnabled = tutorial.Status ?? false;
            DiagnosticEnabled = tutorial.DiagStatus ?? false;
        }

        public CourseTutorialTempData(int tutorialId, string tutorialName, bool learningEnabled, bool diagnosticEnabled)
        {
            TutorialId = tutorialId;
            TutorialName = tutorialName;
            LearningEnabled = learningEnabled;
            DiagnosticEnabled = diagnosticEnabled;
        }

        public int TutorialId { get; set; }
        public string TutorialName { get; set; }
        public bool LearningEnabled { get; set; }
        public bool DiagnosticEnabled { get; set; }
    }
}
