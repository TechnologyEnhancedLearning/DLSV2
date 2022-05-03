namespace DigitalLearningSolutions.Data.Models.Progress
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DetailedCourseProgress : DelegateCourseDetails
    {
        public DetailedCourseProgress(
            Progress progress,
            IEnumerable<DetailedSectionProgress> sections,
            DelegateCourseInfo delegateCourseInfo,
            List<CourseAdminFieldWithAnswer> courseAdminFields,
            AttemptStats attemptStats
        ) : base(delegateCourseInfo, courseAdminFields, attemptStats)
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
}
