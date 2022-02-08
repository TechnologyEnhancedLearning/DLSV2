using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models
{
    public class DetailedCourseProgress
    {
        public DetailedCourseProgress(int? diagnosticScore, IEnumerable<DetailedSectionProgress> sections)
        {
            DiagnosticScore = diagnosticScore;
            Sections = sections;
        }

        public int DelegateId { get; set; }
        public int? DiagnosticScore { get; set; }
        public IEnumerable<DetailedSectionProgress> Sections { get; set; }
    }

    public class DetailedSectionProgress
    {
        public string SectionName { get; set; }
        public int SectionId { get; set; }

        public int Completion { get; set; }
        public int TotalTime { get; set; }
        public int AverageTime { get; set; }
        public bool PostLearningAssessment { get; set; }
        public int? Outcome { get; set; }
        public int? Attempts { get; set; }
        public bool Passed { get; set; }

        public IEnumerable<DetailedTutorialProgress>? Tutorials { get; set; }
    }

    public class DetailedTutorialProgress
    {
        public string TutorialName { get; set; }
        public string TutorialStatus { get; set; }
        public int TimeTaken { get; set; }
        public int AvgTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public int PossibleScore { get; set; }
    }
}
