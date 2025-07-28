namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    public class AddCompetenciesFormData
    {
        public int ID { get; set; }
        public int[] SelectedCompetencyIds { get; set; }
        public int FrameworkId { get; set; }
    }
}
