using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{

    public class ViewSelectedCompetenciesViewModel
    {
        public ViewSelectedCompetenciesViewModel() { }
        public ViewSelectedCompetenciesViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<Competency> competencies, IEnumerable<LinkedFramework> linkedFrameworks)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            Competencies = competencies;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            var competencyCounts = competencies
            .GroupBy(c => c.FrameworkId)
            .ToDictionary(g => g.Key, g => g.Count());
            // Populate LinkedFrameworks with the relevant count
            foreach (var framework in linkedFrameworks)
            {
                if (competencyCounts.TryGetValue(framework.ID, out var count))
                {
                    framework.AssessmentFrameworkCompetencyCount = count;
                }
                else
                {
                    framework.AssessmentFrameworkCompetencyCount = 0;
                }
            }
            LinkedFrameworks = linkedFrameworks;
        }
        public int ID { get; set; }
        
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public IEnumerable<Competency> Competencies { get; set; }
        public IEnumerable<LinkedFramework> LinkedFrameworks { get; set; }
    }
}
