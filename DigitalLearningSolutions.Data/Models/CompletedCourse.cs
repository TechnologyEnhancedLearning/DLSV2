namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class CompletedCourse : BaseCourse
    {
        public DateTime Completed { get; set; }
        public DateTime Evaluated { get; set; }
    }
}
