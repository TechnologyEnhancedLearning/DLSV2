namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.Extensions.Configuration;

    public class AllCompletedItemsPageViewModel
    {
        public readonly IEnumerable<CompletedCourseViewModel> CompletedCourses;

        public AllCompletedItemsPageViewModel(
            IEnumerable<CompletedCourse> completedCourses,
            IConfiguration config
        )
        {
            CompletedCourses = completedCourses.Select(completedCourse =>
                new CompletedCourseViewModel(completedCourse, config)
            );
        }
    }
}
