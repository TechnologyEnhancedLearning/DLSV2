namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class AllAvailableItemsPageViewModel
    {
        public readonly IEnumerable<AvailableCourseViewModel> AvailableCourses;

        public AllAvailableItemsPageViewModel(IEnumerable<AvailableCourse> availableCourses)
        {
            AvailableCourses = availableCourses.Select(availableCourse => new AvailableCourseViewModel(availableCourse));
        }
    }
}
