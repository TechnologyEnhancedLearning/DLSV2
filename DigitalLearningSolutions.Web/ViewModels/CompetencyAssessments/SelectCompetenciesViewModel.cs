using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SelectCompetenciesViewModel
    {
        public SelectCompetenciesViewModel() { }
        public SelectCompetenciesViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<Competency> competencies)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            Competencies = competencies;
        }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }

        public IEnumerable<Competency> Competencies;
    }
}
