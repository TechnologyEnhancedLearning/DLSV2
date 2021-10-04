namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DelegateCourseDetails
    {
        public DelegateCourseDetails(
            DelegateCourseInfo delegateCourseInfo,
            List<CustomPromptWithAnswer> customPrompts,
            AttemptStats attemptStats
        )
        {
            DelegateCourseInfo = delegateCourseInfo;
            CustomPrompts = customPrompts;
            AttemptStats = attemptStats;
        }

        public DelegateCourseInfo DelegateCourseInfo { get; set; }
        public List<CustomPromptWithAnswer> CustomPrompts { get; set; }
        public AttemptStats AttemptStats { get; set; }
    }
}
