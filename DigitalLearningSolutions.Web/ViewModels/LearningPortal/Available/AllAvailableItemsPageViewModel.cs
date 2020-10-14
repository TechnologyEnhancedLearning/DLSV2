namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.Extensions.Configuration;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;

    public class AllAvailableItemsPageViewModel
    {
        public readonly IEnumerable<AvailableCourseViewModel> AvailableCourses;

        public AllAvailableItemsPageViewModel(
            IEnumerable<AvailableCourse> availableCourses,
            IConfiguration config
        )
        {
            AvailableCourses = availableCourses.Select(availableCourse => new AvailableCourseViewModel(availableCourse, config));
        }
    }
}
