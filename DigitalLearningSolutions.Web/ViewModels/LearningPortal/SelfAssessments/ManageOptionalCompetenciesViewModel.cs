namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class ManageOptionalCompetenciesViewModel
    {
        public int SelfAssessmentId { get; set; }
        public string? Vocabulary { get; set; }
        public string? SelfAssessmentName { get; set; }
        public IEnumerable<IGrouping<string, Competency>>? CompetencyGroups { get; set; }
        public List<int>? IncludedSelfAssessmentStructureIds { get; set; }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(Vocabulary);
        }
    }
}
