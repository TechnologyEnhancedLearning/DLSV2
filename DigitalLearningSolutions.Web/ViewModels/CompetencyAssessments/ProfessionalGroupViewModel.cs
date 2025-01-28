namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using System.Collections.Generic;

    public class ProfessionalGroupViewModel
    {
        public IEnumerable<NRPProfessionalGroups> NRPProfessionalGroups { get; set; }
        public CompetencyAssessmentBase CompetencyAssessmentBase { get; set; }
    }
}
