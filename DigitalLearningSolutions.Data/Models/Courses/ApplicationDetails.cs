namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class ApplicationDetails : BaseSearchableItem
    {
        public ApplicationDetails() { }

        public ApplicationDetails(ApplicationDetails applicationDetails)
        {
            ApplicationId = applicationDetails.ApplicationId;
            ApplicationName = applicationDetails.ApplicationName;
            CategoryName = applicationDetails.CategoryName;
            CourseTopicId = applicationDetails.CourseTopicId;
            CourseTopic = applicationDetails.CourseTopic;
            PLAssess = applicationDetails.PLAssess;
            DiagAssess = applicationDetails.DiagAssess;
            CreatedDate = applicationDetails.CreatedDate;
        }

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
        public DateTime CreatedDate { get; set; }
    }
}
