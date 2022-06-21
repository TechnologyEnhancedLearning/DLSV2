using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class FrameworkCompetencyGroupViewModel{
        public FrameworkCompetencyGroupViewModel(FrameworkCompetencyGroup frameworkCompetencyGroup)
        {
            FrameworkCompetencyGroup = frameworkCompetencyGroup;
        }
        public FrameworkCompetencyGroup FrameworkCompetencyGroup { get; set; }
    }
}
