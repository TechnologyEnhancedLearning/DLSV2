namespace DigitalLearningSolutions.Data.Models.DiagnosticAssessment
{
    public class DiagnosticTutorial
    {
        public string TutorialName { get; }
        public int Id { get; }

        public DiagnosticTutorial(string tutorialName, int id)
        {
            TutorialName = tutorialName;
            Id = id;
        }
    }
}
