namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class SelectOptionalCompetenciesViewModel : SelectOptionalCompetenciesFormData
    {
        public SelectOptionalCompetenciesViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<Competency> competencies, bool? taskStatus)
        {
            ID = competencyAssessmentBase.ID;
            TaskStatus = taskStatus;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            MinimumOptionalCompetencies = competencyAssessmentBase.MinimumOptionalCompetencies;
            ManageOptionalCompetenciesPrompt = competencyAssessmentBase.ManageOptionalCompetenciesPrompt;
            UserRole = competencyAssessmentBase.UserRole;
            PublishStatusID = competencyAssessmentBase.PublishStatusID;
            CompetencyGroups = competencies.GroupBy(competency => competency.GroupName);
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            SelectedCompetencyIds = competencies.Where(c => c.Optional == true).Select(c => c.CompetencyID).ToArray();
        }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public int PublishStatusID { get; set; }
        public IEnumerable<IGrouping<string, Competency>>? CompetencyGroups { get; set; }
        public string GetGroupLabel(string? key)
        {
            if (string.IsNullOrWhiteSpace(key)) return "";

            if (string.IsNullOrWhiteSpace(VocabularyPlural))
                return key;
            var keyLower = key.ToLower();
            var vocab = VocabularyPlural.ToLower();
            if (keyLower.Contains(vocab) || keyLower.Contains("proficiencies"))
                return key;
            return $"{key} {vocab}";
        }
    }
}
