namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DelegateCourseDetails
    {
        public DelegateCourseDetails(
            DelegateCourseInfo delegateCourseInfo,
            List<CourseAdminFieldWithAnswer> courseAdminFields,
            AttemptStats attemptStats
        )
        {
            DelegateCourseInfo = delegateCourseInfo;
            CourseAdminFields = courseAdminFields;
            AttemptStats = attemptStats;
        }

        public DelegateCourseInfo DelegateCourseInfo { get; set; }
        public List<CourseAdminFieldWithAnswer> CourseAdminFields { get; set; }
        public AttemptStats AttemptStats { get; set; }
    }
}
