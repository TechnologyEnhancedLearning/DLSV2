namespace DigitalLearningSolutions.Data.Models.Courses
{
    public abstract class BaseCourse : NamedItem
    {
        public string CourseName { get; set; }
        public override string Name
        {
            get => CourseName;
            set => CourseName = value;
        }
        public int CustomisationID { get; set; }

        public override int Id
        {
            get => CustomisationID;
            set => CustomisationID = value;
        }
        public bool HasDiagnostic { get; set; }
        public bool HasLearning { get; set; }
        public bool IsAssessed { get; set; }
    }
}
