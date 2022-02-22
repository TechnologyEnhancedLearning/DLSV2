﻿namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;

    public class DetailedCourseProgress
    {
        public DetailedCourseProgress(Progress progress, IEnumerable<DetailedSectionProgress> sections)
        {
            DiagnosticScore = progress.DiagnosticScore;
            ProgressId = progress.ProgressId;
            CustomisationId = progress.CustomisationId;
            DelegateId = progress.CandidateId;
            Sections = sections;
        }

        public int ProgressId { get; set; }
        public int CustomisationId { get; set; }
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
        public string? PLAssessPath { get; set; }
        public bool IsAssessed { get; set; }
        public int Outcome { get; set; }
        public int Attempts { get; set; }
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
