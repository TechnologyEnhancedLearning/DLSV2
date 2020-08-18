namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class AvailableViewModel
    {
        private readonly IEnumerable<Course> availableCourses;

        public readonly string? BannerText;

        public AvailableViewModel(IEnumerable<Course> availableCourses, string? bannerText)
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
                    Name = c.Name,
                    Id = c.Id
                });
            }
        }

        public class AvailableCourseViewModel
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
    }
}
