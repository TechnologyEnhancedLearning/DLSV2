namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class SetMinimumOptionalCompetenciesViewModel : SetMinimumOptionalCompetenciesFormData
    {
        public SetMinimumOptionalCompetenciesViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<Competency> competencies)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            MinimumOptionalCompetencies = competencyAssessmentBase.MinimumOptionalCompetencies;
            UserRole = competencyAssessmentBase.UserRole;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            OptionalCompetenciesCount = competencies.Count(c => c.Optional == true);
        }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
    }
}
