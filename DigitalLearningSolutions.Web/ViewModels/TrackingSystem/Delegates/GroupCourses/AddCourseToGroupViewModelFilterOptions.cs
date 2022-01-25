namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AddCourseToGroupViewModelFilterOptions
    {
        public static IEnumerable<FilterOptionViewModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            var test = categories.Distinct();
            return test.Select(
                c => new FilterOptionViewModel(
                    c,
                    nameof(CourseAssessmentDetails.CategoryName) + FilteringHelper.Separator +
                    nameof(CourseAssessmentDetails.CategoryName) +
                    FilteringHelper.Separator + c,
                    FilterStatus.Default
                )
            );
        }

        public static IEnumerable<FilterOptionViewModel> GetTopicOptions(IEnumerable<string> topics)
        {
            return topics.Distinct().Select(
                c => new FilterOptionViewModel(
                    c,
                    nameof(CourseAssessmentDetails.CourseTopic) + FilteringHelper.Separator +
                    nameof(CourseAssessmentDetails.CourseTopic) +
                    FilteringHelper.Separator + c,
                    FilterStatus.Default
                )
            );
        }
    }
}
