namespace DigitalLearningSolutions.Data.Models.Progress
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class DetailedCourseProgress : DelegateCourseInfo
    {
        public DetailedCourseProgress(
            Progress progress,
            IEnumerable<DetailedSectionProgress> sections,
            DelegateCourseInfo delegateCourseInfo
        ) : base(delegateCourseInfo)
        {
            DiagnosticScore = progress.DiagnosticScore;
            ProgressId = progress.ProgressId;
            CustomisationId = progress.CustomisationId;
            DelegateId = progress.CandidateId;
            Sections = sections;
        }

        public IEnumerable<DetailedSectionProgress> Sections { get; set; }
    }
}
