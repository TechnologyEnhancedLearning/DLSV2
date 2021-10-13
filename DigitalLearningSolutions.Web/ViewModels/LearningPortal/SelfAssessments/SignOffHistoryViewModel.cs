namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;

    public class SignOffHistoryViewModel
    {
        public CurrentSelfAssessment? SelfAssessment { get; set; }
        public IEnumerable<SupervisorSignOff>? SupervisorSignOffs { get; set; }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(SelfAssessment.Vocabulary);
        }
    }
}
