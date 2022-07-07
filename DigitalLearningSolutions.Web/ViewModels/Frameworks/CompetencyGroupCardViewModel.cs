namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System.Collections.Generic;

    public class CompetencyGroupCardViewModel
    {
        public FrameworkCompetencyGroup FrameworkCompetencyGroup { get; set; }
        public bool CanModify { get; set; }
        public IEnumerable<CompetencyFlag> CompetencyFlags { get; set; }
    }
}
