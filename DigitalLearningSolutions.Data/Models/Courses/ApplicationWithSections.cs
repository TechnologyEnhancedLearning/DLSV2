namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System.Collections.Generic;
    using System.Linq;

    public class ApplicationWithSections : ApplicationDetails
    {
        public ApplicationWithSections(
            ApplicationDetails applicationDetails,
            IEnumerable<Section> sections,
            double popularityRating
        ) : base(applicationDetails)
        {
            Sections = sections;
            TotalMins = Sections.Sum(
                section => section.Tutorials.Sum(
                    tutorial => tutorial.OverrideTutorialMins > 0
                        ? tutorial.OverrideTutorialMins ?? 0
                        : tutorial.AverageTutMins ?? 0
                )
            );
            PopularityRating = popularityRating;
        }

        public int TotalMins { get; set; }
        public double PopularityRating { get; set; }
        public IEnumerable<Section> Sections { get; set; }
    }
}
