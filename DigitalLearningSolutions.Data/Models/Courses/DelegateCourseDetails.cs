namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;
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
            var passRate = attemptStats.totalAttempts == 0
                ? 0
                : Math.Round(100 * attemptStats.attemptsPassed / (double)attemptStats.totalAttempts);
            AttemptStats = (attemptStats.totalAttempts, attemptStats.attemptsPassed, passRate);
        }

        public DelegateCourseInfo DelegateCourseInfo { get; set; }
        public List<CustomPromptWithAnswer> CustomPrompts { get; set; }
        public (int totalAttempts, int attemptsPassed, double passRate) AttemptStats { get; set; }
    }
}
