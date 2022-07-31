namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Utilities;

    public static class CompletedCourseHelper
    {
        private static readonly IClockUtility ClockUtility = new ClockUtility();

        public static CompletedCourse CreateDefaultCompletedCourse(
            int customisationId = 1,
            string courseName = "Course 1",
            bool hasDiagnostic = true,
            bool hasLearning = true,
            bool isAssessed = true,
            int? diagnosticScore = 1,
            int passes = 1,
            int sections = 1,
            int progressId = 1,
            DateTime? evaluated = null,
            DateTime? startedDate = null,
            DateTime? lastAccessed = null,
            DateTime? archivedDate = null,
            DateTime? completed = null
        )
        {
            return new CompletedCourse
            {
                Id = customisationId,
                Name = courseName,
                HasDiagnostic = hasDiagnostic,
                HasLearning = hasLearning,
                IsAssessed = isAssessed,
                DiagnosticScore = diagnosticScore,
                Passes = passes,
                Sections = sections,
                ProgressID = progressId,
                Evaluated = evaluated,
                StartedDate = startedDate ?? ClockUtility.UtcNow,
                LastAccessed = lastAccessed ?? ClockUtility.UtcNow,
                Completed = completed ?? ClockUtility.UtcNow,
                ArchivedDate = archivedDate,
            };
        }
    }
}
