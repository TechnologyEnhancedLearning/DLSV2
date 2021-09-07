﻿namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class CourseNameInfo : BaseSearchableItem
    {
        public int CustomisationId { get; set; }
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
