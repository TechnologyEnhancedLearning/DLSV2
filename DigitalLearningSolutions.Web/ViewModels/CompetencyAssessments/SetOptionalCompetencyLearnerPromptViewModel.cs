namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class SetOptionalCompetencyLearnerPromptViewModel : SetOptionalCompetencyLearnerPromptFormData
    {
        public SetOptionalCompetencyLearnerPromptViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<Competency> competencies)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            ManageOptionalCompetenciesPrompt = competencyAssessmentBase.ManageOptionalCompetenciesPrompt;
            UserRole = competencyAssessmentBase.UserRole;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
        }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
    }
}
