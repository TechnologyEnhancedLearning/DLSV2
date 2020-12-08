namespace DigitalLearningSolutions.Data.Models.TutorialContent
{
    public class TutorialContent
    {
        public int Id { get; }
        public int CandidateID { get; }
        public int SectionID { get; }
        public int CustomisationID { get; }
        public string TutorialName { get; }
        public string Status { get; }
        public int TimeSpent { get; }
        public int AverageTutMins { get; }
        public int TutScore { get; }
        public int PossScore { get; }
        public bool DiagStatus { get; }
        public int DiagAttempts { get; }
        public string Objectives { get; }
        public string VideoPath { get; }
        public string TutorialPath { get; }
        public string SupportingMatsPath { get; }

        public TutorialContent(
            int id,
            int candidateId,
            int sectionId,
            int customisationId,
            string tutorialName,
            string status,
            int timeSpent,
            int averageTutMins,
            int tutScore,
            int possScore,
            bool diagStatus,
            int diagAttempts,
            string objectives,
            string videoPath,
            string tutorialPath,
            string supportingMatsPath
        )
        {
            Id = id;
            CandidateID = candidateId;
            SectionID = sectionId;
            CustomisationID = customisationId;
            TutorialName = tutorialName;
            Status = status;
            TimeSpent = timeSpent;
            AverageTutMins = averageTutMins;
            TutScore = tutScore;
            PossScore = possScore;
            DiagStatus = diagStatus;
            DiagAttempts = diagAttempts;
            Objectives = objectives;
            VideoPath = videoPath;
            TutorialPath = tutorialPath;
            SupportingMatsPath = supportingMatsPath;
        }
    }
}
