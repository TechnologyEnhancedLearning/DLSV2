namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DocumentFormat.OpenXml.Bibliography;

    public class DetailedCourseProgressViewModel
    {
        public DetailedCourseProgressViewModel(DelegateUser delegateUser, DetailedCourseProgress progress)
        {
            DelegateName = delegateUser.FullName;
            DelegateEmail = delegateUser.EmailAddress;
            DelegateNumber = delegateUser.CandidateNumber;
            // TODO HEEDLS-567 what's up with the datetime properties
            // TODO HEEDLS-567 what to do with this if it's null?
            DiagnosticScore = progress.DiagnosticScore;

            Sections = progress.Sections.Select(s => new SectionProgressViewModel(s));
        }

        public string DelegateName { get; set; }
        public string DelegateEmail { get; set; }
        public string DelegateNumber { get; set; }
        public DateTime LastAccess { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime CompleteBy { get; set; }
        public int DiagnosticScore { get; set; }

        public IEnumerable<SectionProgressViewModel> Sections { get; set; }
    }

    public class SectionProgressViewModel
    {
        public SectionProgressViewModel(DetailedSectionProgress section)
        {
            SectionName = section.SectionName;
            Completion = section.Completion;
            TotalTime = section.TotalTime;
            AverageTime = section.AverageTime;
            PostLearningAssessment = section.PostLearningAssessment;
            Outcome = section.Outcome;
            Attempts = section.Attempts;
            Passed = section.Passed;

            Tutorials = section.Tutorials.Select(t => new TutorialProgressViewModel(t));
        }

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
        public TutorialProgressViewModel(DetailedTutorialProgress tutorial)
        {
            TutorialName = tutorial.TutorialName;
            TutorialStatus = tutorial.TutorialStatus;
            TimeTaken = tutorial.TimeTaken;
            AvgTime = tutorial.AvgTime;
            // TODO HEEDLS-567 what to do with this if it's null?
            DiagnosticScore = tutorial.DiagnosticScore;
        }

        public string TutorialName { get; set; }
        public string TutorialStatus { get; set; }
        public int TimeTaken { get; set; }
        public int AvgTime { get; set; }
        public int DiagnosticScore { get; set; }
    }
}
