using DigitalLearningSolutions.Data.Models.PostLearningAssessment;
using DigitalLearningSolutions.Data.Models.Progress;
using DigitalLearningSolutions.Data.Models;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using DigitalLearningSolutions.Data.Models.Courses;
using DigitalLearningSolutions.Web.Models.Enums;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    public class PreviewProgressViewModel
    {
        public PreviewProgressViewModel(DelegateCourseProgressInfo courseProgressInfo, DelegateAccessRoute accessedVia)
        {
            ProgressId = courseProgressInfo.ProgressId;
            CandidateName = courseProgressInfo.CandidateName;
            CandidateNumber = courseProgressInfo.CandidateNumber;
            CourseName = courseProgressInfo.Course;
            CourseStatus = courseProgressInfo.Completed != null ? "COMPLETE (Completed: " + courseProgressInfo.Completed?.ToString("dd MMM yyyy") + ")" : "INCOMPLETE";
            DiganosticScorePercent = courseProgressInfo.DiagnosticScore;
            LearningCompletedPercent = courseProgressInfo.LearningDone;
            AssessmentsPassed = courseProgressInfo.PLPasses.ToString() + " out of " + courseProgressInfo.Sections.ToString();
            IsAssessed = courseProgressInfo.IsAssessed;
            TutCompletionThreshold = courseProgressInfo.TutCompletionThreshold;
            DiagCompletionThreshold = courseProgressInfo.DiagCompletionThreshold;
            SectionDetails = courseProgressInfo.SectionProgress;
            AccessedVia = accessedVia;
        }
        public int ProgressId { get; set; }
        public string CandidateName { get; set; }
        public string CandidateNumber { get; set; }
        public string CourseName { get; set; }
        public string CourseStatus { get; set; }
        public int? DiganosticScorePercent { get; set; }
        public int LearningCompletedPercent { get; set; }
        public string AssessmentsPassed { get; set; }
        public bool IsAssessed { get; set; }
        public int TutCompletionThreshold { get; set; }
        public int DiagCompletionThreshold { get; set; }
        public IEnumerable<SectionProgress> SectionDetails { get; set; }
        public DelegateAccessRoute AccessedVia { get; set; }
    }
}
