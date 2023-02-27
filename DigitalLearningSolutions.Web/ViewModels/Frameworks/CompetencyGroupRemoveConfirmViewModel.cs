using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyGroupRemoveConfirmViewModel
    {
        public CompetencyGroupRemoveConfirmViewModel(int frameworkId, int frameworkCompetencyGroupId, int competencyGroupId, int competencyCount)
        {
            FrameworkId = frameworkId;
            FrameworkCompetencyGroupId = frameworkCompetencyGroupId;
            CompetencyGroupId = competencyGroupId;
            CompetencyCount = competencyCount;
        }
        public int FrameworkId { get; set; }
        public int FrameworkCompetencyGroupId { get; set; }
        public int CompetencyGroupId { get; set; }
        public int CompetencyCount { get; set; }
    }
}
