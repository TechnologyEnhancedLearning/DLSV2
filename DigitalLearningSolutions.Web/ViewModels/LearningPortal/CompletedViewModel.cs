namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class CompletedViewModel
    {
        private readonly IEnumerable<Course> completedCourses;
        public readonly string? BannerText;

        public CompletedViewModel(IEnumerable<Course> completedCourses, string? bannerText)
        {
            this.completedCourses = completedCourses;
            BannerText = bannerText;
        }

        public IEnumerable<CompletedCourseViewModel> CompletedCourses
        {
            get
            {
                return completedCourses.Select(c => new CompletedCourseViewModel
                {
                    Name = c.Name,
                    Id = c.Id
                });
            }
        }

        public class CompletedCourseViewModel
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
    }
}
