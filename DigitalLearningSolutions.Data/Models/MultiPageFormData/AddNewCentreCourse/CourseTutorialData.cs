namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    public class CourseTutorialData
    {
        // 'Unused' constructor required for JsonConvert
        public CourseTutorialData(){}

        public CourseTutorialData(Tutorial tutorial)
        {
            TutorialId = tutorial.TutorialId;
            TutorialName = tutorial.TutorialName;
            LearningEnabled = tutorial.Status ?? false;
            DiagnosticEnabled = tutorial.DiagStatus ?? false;
        }

        public CourseTutorialData(int tutorialId, string tutorialName, bool learningEnabled, bool diagnosticEnabled)
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
