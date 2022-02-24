namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DelegateCourseDetails
    {
        public DelegateCourseDetails(
            DelegateCourseInfo delegateCourseInfo,
            List<CoursePromptWithAnswer> coursePrompts,
            AttemptStats attemptStats
        )
        {
            DelegateCourseInfo = delegateCourseInfo;
            CoursePrompts = coursePrompts;
            AttemptStats = attemptStats;
        }

        public DelegateCourseInfo DelegateCourseInfo { get; set; }
        public List<CoursePromptWithAnswer> CoursePrompts { get; set; }
        public AttemptStats AttemptStats { get; set; }
    }
}
