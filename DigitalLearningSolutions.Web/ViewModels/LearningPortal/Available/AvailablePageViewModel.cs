namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.Extensions.Configuration;

    public class AvailablePageViewModel
    {
        public readonly IEnumerable<AvailableCourseViewModel> AvailableCourses;

        public readonly string? BannerText;

        public AvailablePageViewModel(IEnumerable<AvailableCourse> availableCourses, IConfiguration config, string? bannerText)
        {
            AvailableCourses = availableCourses.Select(c => new AvailableCourseViewModel(c, config));
            BannerText = bannerText;
        }
    }
}
