namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class CategoryTopicFilterViewModel : FilterModel
    {
        public CategoryTopicFilterViewModel(
            string filterProperty,
            string filterName,
            IEnumerable<FilterOptionModel> filterOptions,
            string actionParameterName,
            string? categoryFilterString,
            string? topicFilterString
        ) : base(filterProperty, filterName, filterOptions)
        {
            ActionParameterName = actionParameterName;
            CategoryFilterString = categoryFilterString;
            TopicFilterString = topicFilterString;
        }

        public string ActionParameterName { get; }
        public string? CategoryFilterString { get; }
        public string? TopicFilterString { get; }
    }
}
