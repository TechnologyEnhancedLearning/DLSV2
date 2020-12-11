namespace DigitalLearningSolutions.Data.Models.SectionContent
{
    using System.Collections.Generic;

    public class SectionContent
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public int TimeMins { get; }
        public int AverageSectionTime { get; }
        public bool HasLearning { get; }
        public double PercentComplete { get; }
        public int DiagAttempts { get; }
        public int SecScore { get; }
        public int SecOutOf { get; }
        public string DiagAssessPath { get; }
        public string PLAssessPath { get; }
        public int AttemptsPL { get; }
        public int PLPassed { get; }
        public bool DiagStatus { get; }
        public bool IsAssessed { get; }
        public List<SectionTutorial> Tutorials { get; } = new List<SectionTutorial>();

        public SectionContent(
            string applicationName,
            string customisationName,
            string sectionName,
            int timeMins,
            int averageSectionTime,
            bool hasLearning,
            double percentComplete,
            int diagAttempts,
            int secScore,
            int secOutOf,
            string diagAssessPath,
            string plAssessPath,
            int attemptsPl,
            int plPassed,
            bool diagStatus,
            bool isAssessed)
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            SectionName = sectionName;
            TimeMins = timeMins;
            AverageSectionTime = averageSectionTime;
            HasLearning = hasLearning;
            PercentComplete = percentComplete;
            DiagAttempts = diagAttempts;
            SecScore = secScore;
            SecOutOf = secOutOf;
            DiagAssessPath = diagAssessPath;
            PLAssessPath = plAssessPath;
            AttemptsPL = attemptsPl;
            PLPassed = plPassed;
            DiagStatus = diagStatus;
            IsAssessed = isAssessed;
        }
    }
}
