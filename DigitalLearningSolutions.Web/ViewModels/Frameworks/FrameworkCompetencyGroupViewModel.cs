using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class FrameworkCompetencyGroupViewModel{
        public FrameworkCompetencyGroupViewModel(FrameworkCompetencyGroup frameworkCompetencyGroup)
        {
            Ordering = frameworkCompetencyGroup.Ordering;
            FrameworkCompetencyGroup = frameworkCompetencyGroup;
        }

        public int Ordering { get; set; }
        public FrameworkCompetencyGroup FrameworkCompetencyGroup { get; set; }
    }
}
