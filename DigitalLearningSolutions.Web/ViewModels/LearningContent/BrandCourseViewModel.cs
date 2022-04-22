namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
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
            CategoryName = application.CategoryName;
            CourseTopic = application.CourseTopic;
            CreatedDate = application.CreatedDate;
            Sections = application.Sections.Select(s => new BrandCourseSectionViewModel(s));
        }

        public string DisplayTime { get; set; }
        public int Time { get; set; }
        public double PopularityRating { get; set; }
        public IEnumerable<BrandCourseSectionViewModel> Sections { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }

        public string CategoryName { get; set; }
        public string CourseTopic { get; set; }

        public string CategoryFilter => nameof(ApplicationWithSections.CategoryName) + FilteringHelper.Separator +
                                        nameof(ApplicationWithSections.CategoryName) +
                                        FilteringHelper.Separator + CategoryName;

        public string TopicFilter => nameof(ApplicationWithSections.CourseTopic) + FilteringHelper.Separator +
                                     nameof(ApplicationWithSections.CourseTopic) +
                                     FilteringHelper.Separator + CourseTopic;

        public DateTime CreatedDate { get; set; }
    }
}
