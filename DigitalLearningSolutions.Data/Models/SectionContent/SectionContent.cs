namespace DigitalLearningSolutions.Data.Models.SectionContent
{
    using System.Collections.Generic;

    public class SectionContent
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public int SectionTime { get; set; }
        public int AverageSectionTime { get; }
        public bool HasLearning { get; }
        public int DiagnosticAttempts { get; set; }
        public int SectionScore { get; set; }
        public int MaxSectionScore { get; set; }
        public string? DiagnosticAssessmentPath { get; }
        public string? PostLearningAssessmentPath { get; }
        public int PostLearningAttempts { get; }
        public bool PostLearningPassed { get; }
        public bool DiagnosticStatus { get; }
        public bool IsAssessed { get; }
        public List<SectionTutorial> Tutorials { get; } = new List<SectionTutorial>();

        public SectionContent(
            string applicationName,
            string customisationName,
            string sectionName,
            int tutTime,
            int averageSectionTime,
            bool hasLearning,
            int diagAttempts,
            int diagLast,
            int diagAssessOutOf,
            string? diagAssessPath,
            string? plAssessPath,
            int attemptsPl,
            int plPasses,
            bool diagStatus,
            bool isAssessed)
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            SectionName = sectionName;
            SectionTime = tutTime;
            AverageSectionTime = averageSectionTime;
            HasLearning = hasLearning;
            DiagnosticAttempts = diagAttempts;
            SectionScore = diagLast;
            MaxSectionScore = diagAssessOutOf;
            DiagnosticAssessmentPath = diagAssessPath;
            PostLearningAssessmentPath = plAssessPath;
            PostLearningAttempts = attemptsPl;
            PostLearningPassed = plPasses > 0;
            DiagnosticStatus = diagStatus;
            IsAssessed = isAssessed;
        }
    }
}
