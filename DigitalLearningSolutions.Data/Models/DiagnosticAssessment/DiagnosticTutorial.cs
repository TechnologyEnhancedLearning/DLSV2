namespace DigitalLearningSolutions.Data.Models.DiagnosticAssessment
{
    public class DiagnosticTutorial
    {
        public string TutorialName { get; }
        public int Id { get; }
        public bool IsDisplayed { get; }

        public DiagnosticTutorial(string tutorialName, int id, bool status)
        {
            TutorialName = tutorialName;
            Id = id;
            IsDisplayed = status;
        }
    }
}
