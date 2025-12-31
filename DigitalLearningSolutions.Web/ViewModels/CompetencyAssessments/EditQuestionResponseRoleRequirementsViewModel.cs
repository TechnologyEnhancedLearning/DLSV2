using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class EditQuestionResponseRoleRequirementsViewModel : EditQuestionResponseRoleRequirementsFormData
    {
        public EditQuestionResponseRoleRequirementsViewModel(
            CompetencyAssessmentBase competencyAssessmentBase,
            List<GroupedCompetencyWithAssessmentRoleRequirements> groupedCompetencyWithAssessmentRoleRequirements,
            int countAssessmentQuestionInSelfAssessment
            )
        {
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            Id = competencyAssessmentBase.ID;
            GroupedCompetencyWithAssessmentRoleRequirements = groupedCompetencyWithAssessmentRoleRequirements;
            CountAssessmentQuestionInSelfAssessment = countAssessmentQuestionInSelfAssessment;
        }
        public int Id { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public int CountAssessmentQuestionInSelfAssessment { get; set; }
        public List<GroupedCompetencyWithAssessmentRoleRequirements> GroupedCompetencyWithAssessmentRoleRequirements { get; set; }
    }
}
