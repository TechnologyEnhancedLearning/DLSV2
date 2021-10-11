namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class ManageOptionalCompetenciesViewModel
    {
        public CurrentSelfAssessment? SelfAssessment { get; set; }
        public IEnumerable<IGrouping<string, Competency>>? CompetencyGroups { get; set; }
        public List<int>? IncludedSelfAssessmentStructureIds { get; set; }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(SelfAssessment.Vocabulary);
        }
    }
}
