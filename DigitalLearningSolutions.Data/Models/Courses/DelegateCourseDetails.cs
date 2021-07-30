namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DelegateCourseDetails
    {
        public DelegateCourseDetails(
            DelegateCourseInfo delegateCourseInfo,
            List<CustomPromptWithAnswer> customPrompts,
            (int totalAttempts, int attemptsPassed) attemptStats
        )
        {
            DelegateCourseInfo = delegateCourseInfo;
            CustomPrompts = customPrompts;
            AttemptStats = attemptStats;
        }

        public DelegateCourseInfo DelegateCourseInfo { get; set; }
        public List<CustomPromptWithAnswer> CustomPrompts { get; set; }
        public (int totalAttempts, int attemptsPassed) AttemptStats { get; set; }
    }
}
