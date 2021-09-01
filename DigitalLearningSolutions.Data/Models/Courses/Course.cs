namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class Course : BaseSearchableItem
    {
        public int CustomisationId { get; set; }
        public int CentreId { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }
        public bool Active { get; set; }

        public string CourseName => string.IsNullOrWhiteSpace(CustomisationName)
            ? ApplicationName
            : ApplicationName + " - " + CustomisationName;

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? CourseName;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
