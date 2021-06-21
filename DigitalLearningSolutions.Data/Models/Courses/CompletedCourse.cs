namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class CompletedCourse : StartedLearningItem
    {
        public DateTime Completed { get; set; }
        public DateTime? Evaluated { get; set; }
        public DateTime? ArchivedDate { get; set; }
    }
}
