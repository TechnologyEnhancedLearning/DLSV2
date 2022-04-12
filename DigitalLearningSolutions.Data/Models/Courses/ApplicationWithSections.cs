namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class ApplicationWithSections : ApplicationDetails
    {
        public ApplicationWithSections(
            ApplicationDetails applicationDetails,
            IEnumerable<Section> sections,
            double popularityRating)
        {
            ApplicationId = applicationDetails.ApplicationId;
            ApplicationName = applicationDetails.ApplicationName;
            CategoryName = applicationDetails.CategoryName;
            CourseTopicId = applicationDetails.CourseTopicId;
            CourseTopic = applicationDetails.CourseTopic;
            PLAssess = applicationDetails.PLAssess;
            DiagAssess = applicationDetails.DiagAssess;
            Sections = sections;
            TotalMins = 0;
            foreach (var section in Sections)
            {
                foreach (var tutorial in section.Tutorials)
                {
                    if (tutorial.OverrideTutorialMins > 0)
                    {
                        TotalMins += tutorial.OverrideTutorialMins ?? 0;
                    }
                    else
                    {
                        TotalMins += tutorial.AverageTutMins ?? 0;
                    }
                }
            }

            PopularityRating = popularityRating;
        }
        public int TotalMins { get; set; }
        public double PopularityRating { get; set; }
        public IEnumerable<Section> Sections { get; set; }
    }
}
