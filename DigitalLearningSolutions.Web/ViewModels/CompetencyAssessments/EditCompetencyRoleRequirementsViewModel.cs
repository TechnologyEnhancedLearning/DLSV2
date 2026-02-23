using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class EditCompetencyRoleRequirementsViewModel
    {
        public EditCompetencyRoleRequirementsViewModel(
            CompetencyAssessmentBase competencyAssessmentBase,
            List<GroupedCompetencyWithAssessmentRoleRequirements> groupedCompetencyWithAssessmentRoleRequirements
            )
        {
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            Id = competencyAssessmentBase.ID;
            GroupedCompetencyWithAssessmentRoleRequirements = groupedCompetencyWithAssessmentRoleRequirements;
        }
        public int Id { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public List<GroupedCompetencyWithAssessmentRoleRequirements> GroupedCompetencyWithAssessmentRoleRequirements { get; set; }

    }
}

