namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public abstract class BaseCourse : NamedItem
    {
        public string CourseName { get; set; }
        public override string Name
        {
            get => CourseName;
            set => CourseName = value;
        }
        public int CustomisationID { get; set; }
        public bool HasDiagnostic { get; set; }
        public bool HasLearning { get; set; }
        public bool IsAssessed { get; set; }
        public DateTime StartedDate { get; set; }
        public int? DiagnosticScore { get; set; }
        public int Passes { get; set; }
        public int Sections { get; set; }
        public int ProgressID { get; set; }
    }
}
