namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using Microsoft.Extensions.Configuration;

    public class AllCompletedItemsPageViewModel
    {
        public readonly IEnumerable<CompletedLearningItemViewModel> CompletedActivities;

        public AllCompletedItemsPageViewModel(
            IEnumerable<CompletedCourse> completedCourses,
            IEnumerable<CompletedActionPlanResource> completedResource,
            IConfiguration config
        )
        {
            CompletedActivities = completedCourses.Select(
                completedCourse =>
                    new CompletedCourseViewModel(completedCourse, config)
            );
            foreach (var resource in completedResource)
            {
                CompletedActivities = CompletedActivities.Append(new CompletedLearningResourceViewModel(resource));
            }
        }
    }
}
