namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;

    public class AddCompetenciesSelectFrameworkViewModel
    {
        public AddCompetenciesSelectFrameworkViewModel(
           CompetencyAssessmentBase competencyAssessmentBase,
           IEnumerable<BaseFramework> linkedFrameworks
       )
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            LinkedFrameworks = linkedFrameworks;
            UserRole = competencyAssessmentBase.UserRole;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
        }
        public int ID { get; set; }
        public string? CompetencyAssessmentName { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public int FrameworkId { get; set; }
        public IEnumerable<BaseFramework> LinkedFrameworks { get; set; }
    }
}
