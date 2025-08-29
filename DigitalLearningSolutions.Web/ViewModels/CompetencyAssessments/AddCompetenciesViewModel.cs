namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    public class AddCompetenciesViewModel
    {
        public AddCompetenciesViewModel(CompetencyAssessmentBase competencyAssessmentBase, IEnumerable<FrameworkCompetencyGroup> groupedCompetencies, IEnumerable<FrameworkCompetency> ungroupedCompetencies, int frameworkId, string? frameworkName, int[] selectedFrameworkCompetencies)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            UserRole = competencyAssessmentBase.UserRole;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            GroupedCompetencies = groupedCompetencies;
            UngroupedCompetencies = ungroupedCompetencies;
            FrameworkId = frameworkId;
            FrameworkName = frameworkName;
            SelectedCompetencyIds = selectedFrameworkCompetencies;
        }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public IEnumerable<FrameworkCompetencyGroup> GroupedCompetencies { get; set; }
        public IEnumerable<FrameworkCompetency> UngroupedCompetencies { get; set; }
        public int[] SelectedCompetencyIds { get; set; }
        public int FrameworkId { get; set; }
        public string? FrameworkName { get; set; }
    }
}
