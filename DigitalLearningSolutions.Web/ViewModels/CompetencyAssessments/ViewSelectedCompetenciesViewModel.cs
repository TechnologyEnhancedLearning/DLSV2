using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ViewSelectedCompetenciesViewModel
    {
        public ViewSelectedCompetenciesViewModel() { }
        public ViewSelectedCompetenciesViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<Competency> competencies)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            Competencies = competencies;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
        }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }

        public IEnumerable<Competency> Competencies;
    }
}
