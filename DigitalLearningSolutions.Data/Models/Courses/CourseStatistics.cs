﻿namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class CourseStatistics : Course
    {
        public bool AllCentres { get; set; }
        public new int DelegateCount { get; set; }
        public int CompletedCount { get; set; }
        public int InProgressCount => DelegateCount - CompletedCount;
        public int AllAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public bool HideInLearnerPortal { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CourseTopic { get; set; } = string.Empty;
        public string LearningMinutes { get; set; } = string.Empty;
        public bool IsAssessed { get; set; }

        public double PassRate => AllAttempts == 0 ? 0 : Math.Round(100 * AttemptsPassed / (double)AllAttempts);
    }
}
