namespace DigitalLearningSolutions.Data.Models.Courses
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class CourseNameInfo : BaseSearchableItem
    {
        public CourseNameInfo() { }

        public CourseNameInfo(CourseNameInfo courseNameInfo)
        {
            CustomisationName = courseNameInfo.CustomisationName;
            ApplicationName = courseNameInfo.ApplicationName;
        }

        public string CustomisationName { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;

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
