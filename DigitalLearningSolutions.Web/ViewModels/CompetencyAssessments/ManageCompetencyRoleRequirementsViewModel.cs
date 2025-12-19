using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ManageCompetencyRoleRequirementsViewModel
    {
        public ManageCompetencyRoleRequirementsViewModel(
            CompetencyAssessmentBase competencyAssessmentBase,
            List<GroupedCompetencyWithAssessmentRoleRequirements> groupedCompetencyWithAssessmentRoleRequirements
            )
        {
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            EnforceRoleRequirementsForSignOff = competencyAssessmentBase.EnforceRoleRequirementsForSignOff;
            IncludeRequirementsFilters = competencyAssessmentBase.IncludeRequirementsFilters;
            Id = competencyAssessmentBase.ID;
            CountCompetencyRequirements = groupedCompetencyWithAssessmentRoleRequirements.SelectMany(g => g.Competencies)
    .SelectMany(c => c.Questions)
    .Count(q => q.Responses.Any());
        }
        public int Id { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public bool EnforceRoleRequirementsForSignOff { get; set; }
        public bool IncludeRequirementsFilters { get; set; }
        public int CountCompetencyRequirements { get; set; }
       
    }
}
