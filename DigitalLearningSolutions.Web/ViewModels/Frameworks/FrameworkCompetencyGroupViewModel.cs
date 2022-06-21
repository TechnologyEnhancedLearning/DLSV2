using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class FrameworkCompetencyGroupViewModel{
        public FrameworkCompetencyGroupViewModel(FrameworkCompetencyGroup frameworkCompetencyGroup)
        {
            Ordering = frameworkCompetencyGroup.Ordering;
            FrameworkCompetencies = frameworkCompetencyGroup.FrameworkCompetencies;
        }

        public int Ordering { get; set; }
        public List<FrameworkCompetency> FrameworkCompetencies { get; set; }
    }
}
