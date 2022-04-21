namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;

    public class BrandCourseViewModel
    {
        public BrandCourseViewModel(ApplicationWithSections application)
        {
            ApplicationId = application.ApplicationId;
            ApplicationName = application.ApplicationName;
            PopularityRating = application.PopularityRating;
            DisplayTime = DisplayStringHelper.GetTimeStringFromMinutes(application.TotalMins);
            Time = application.TotalMins;
            Sections = application.Sections.Select(s => new BrandCourseSectionViewModel(s));
        }

        public string DisplayTime { get; set; }
        public int Time { get; set; }
        public double PopularityRating { get; set; }
        public IEnumerable<BrandCourseSectionViewModel> Sections { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
    }
}
