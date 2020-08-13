namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Extensions.Configuration;

    public class CompletedViewModel
    {
        private readonly IEnumerable<CompletedCourse> completedCourses;
        private readonly IConfiguration config;
        public readonly string? BannerText;

        public CompletedViewModel(
            IEnumerable<CompletedCourse> completedCourses,
            IConfiguration config,
            string? bannerText
        )
        {
            this.config = config;
            BannerText = bannerText;
            this.completedCourses = completedCourses;
        }

        public IEnumerable<CompletedCourseViewModel> CompletedCourses
        {
            get
            {
                return completedCourses.Select(c => new CompletedCourseViewModel(c, config));
            }
        }
    }
}
