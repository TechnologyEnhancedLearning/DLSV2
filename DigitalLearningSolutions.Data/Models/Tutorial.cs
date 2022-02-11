namespace DigitalLearningSolutions.Data.Models
{
    public class Tutorial
    {
        public Tutorial() { }

        public Tutorial(int tutorialId, string tutorialName, bool? status, bool? diagStatus)
        {
            TutorialId = tutorialId;
            TutorialName = tutorialName;
            DiagStatus = diagStatus;
            Status = status;
        }

        public int TutorialId { get; set; }
        public string TutorialName { get; set; }
        public bool? Status { get; set; }
        public bool? DiagStatus { get; set; }
    }
}
