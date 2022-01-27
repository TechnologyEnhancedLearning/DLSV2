namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System;
    using System.Collections.Generic;

    public class DetailedCourseProgressViewModel
    {
        public string DelegateName { get; set; }
        public string DelegateEmail { get; set; }
        public string DelegateId { get; set; }
        public DateTime LastAccess { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime CompleteBy { get; set; }
        public int DiagnosticScore { get; set; }

        public IEnumerable<SectionProgressViewModel> Sections { get; set; }
    }

    public class SectionProgressViewModel
    {
        public string SectionName { get; set; }
        public int Completion { get; set; }
        public int TotalTime { get; set; }
        public int AverageTime { get; set; }
        public bool PostLearningAssessment { get; set; }
        public int? Outcome { get; set; }
        public int? Attempts { get; set; }
        public bool Passed { get; set; }

        public IEnumerable<TutorialProgressViewModel> Tutorials { get; set; }
    }

    public class TutorialProgressViewModel
    {
        public string TutorialName { get; set; }
        public string TutorialStatus { get; set; }
        public int TimeTaken { get; set; }
        public int AvgTime { get; set; }
        public int DiagnosticScore { get; set; }
    }
}
