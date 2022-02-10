namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class LearningLog
    {
        public LearningLog(DelegateCourseInfo delegateCourseInfo, IEnumerable<LearningLogEntry> entries)
        {
            DelegateCourseInfo = delegateCourseInfo;
            Entries = entries;
        }

        public DelegateCourseInfo DelegateCourseInfo { get; set; }
        public IEnumerable<LearningLogEntry> Entries { get; set; }
    }
}
