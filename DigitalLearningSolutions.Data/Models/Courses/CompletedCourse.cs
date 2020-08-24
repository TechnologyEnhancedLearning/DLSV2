namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class CompletedCourse : StartedCourse
    {
        public DateTime Completed { get; set; }
        public DateTime? Evaluated { get; set; }
    }
}
