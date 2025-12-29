using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class EditRoleRequirementsFlagsViewModel : ManageCompetencyRoleRequirementsFormData
    {
        public EditRoleRequirementsFlagsViewModel(
            CompetencyAssessmentBase competencyAssessmentBase
            )
        {
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            TaskStatus = false;
            Id = competencyAssessmentBase.ID;
            EnforceRoleRequirementsForSignOff = competencyAssessmentBase.EnforceRoleRequirementsForSignOff;
            IncludeRequirementsFilters = competencyAssessmentBase.IncludeRequirementsFilters;
        }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
    }
}
