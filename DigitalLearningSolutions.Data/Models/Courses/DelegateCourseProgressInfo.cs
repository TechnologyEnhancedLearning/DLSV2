using DigitalLearningSolutions.Data.Models.Progress;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class DelegateCourseProgressInfo : DelegateCourseInfo
    {
        public string CentreName { get; set; }
        public string CandidateName { get; set; }
        public string Course { get; set; }
        public int DiagnosticAttempts { get; set; }
        public int TotalTime { get; set; }
        public int LearningDone { get; set; }
        public int PLAttempts { get; set; }
        public int PLPasses { get; set; }
        public int Sections { get; set; }
        public bool IsAssessed { get; set; }
        public int TutCompletionThreshold { get; set; }
        public int DiagCompletionThreshold { get; set; }
        public int AssessAttempts { get; set; }
        public int PLAPassThreshold { get; set; }
        public IEnumerable<SectionProgress>? SectionProgress { get; set; }

    }
}
