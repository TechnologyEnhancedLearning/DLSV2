﻿namespace DigitalLearningSolutions.Data.Models.Courses
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class ApplicationDetails : BaseSearchableItem
    {
        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? ApplicationName;
            set => SearchableNameOverrideForFuzzySharp = value;
        }

        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string CategoryName { get; set; }
        public int CourseTopicId { get; set; }
        public string CourseTopic { get; set; }
        public bool PLAssess { get; set; }
        public bool DiagAssess { get; set; }
    }
}
