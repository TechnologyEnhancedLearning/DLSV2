namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class CategoryTopicFilterViewModel : FilterViewModel
    {
        public CategoryTopicFilterViewModel(
            string filterProperty,
            string filterName,
            IEnumerable<FilterOptionViewModel> filterOptions,
            string actionParameterName,
            string? categoryFilterBy,
            string? topicFilterBy
        ) : base(filterProperty, filterName, filterOptions)
        {
            ActionParameterName = actionParameterName;
            CategoryFilterBy = categoryFilterBy;
            TopicFilterBy = topicFilterBy;
        }

        public string ActionParameterName { get; }
        public string? CategoryFilterBy { get; }
        public string? TopicFilterBy { get; }
    }
}
