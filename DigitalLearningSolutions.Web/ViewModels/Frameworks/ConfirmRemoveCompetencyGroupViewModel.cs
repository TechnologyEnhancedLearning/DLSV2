using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class ConfirmRemoveCompetencyGroupViewModel{
        public ConfirmRemoveCompetencyGroupViewModel(int frameworkId ,int frameworkCompetencyGroupId)
        {
            FrameworkId = frameworkId;
            FrameworkCompetencyGroupId = frameworkCompetencyGroupId;
        }
        public int FrameworkId { get; set; }
        public int FrameworkCompetencyGroupId { get; set; }
    }
}
