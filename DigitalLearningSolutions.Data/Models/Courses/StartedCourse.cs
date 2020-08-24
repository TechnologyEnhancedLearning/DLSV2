namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public abstract class StartedCourse : BaseCourse
    {
        public DateTime StartedDate { get; set; }
        public DateTime LastAccessed { get; set; }
        public int? DiagnosticScore { get; set; }
        public int Passes { get; set; }
        public int Sections { get; set; }
        public int ProgressID { get; set; }
    }
}
