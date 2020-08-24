namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class AvailableViewModel
    {
        private readonly IEnumerable<AvailableCourse> availableCourses;

        public readonly string? BannerText;

        public AvailableViewModel(IEnumerable<AvailableCourse> availableCourses, string? bannerText)
        {
            this.availableCourses = availableCourses;
            BannerText = bannerText;
        }

        public IEnumerable<AvailableCourseViewModel> AvailableCourses
        {
            get
            {
                return availableCourses.Select(c => new AvailableCourseViewModel
                {
                    Name = c.Name
                });
            }
        }

        public class AvailableCourseViewModel
        {
            public string Name { get; set; }
        }
    }
}
