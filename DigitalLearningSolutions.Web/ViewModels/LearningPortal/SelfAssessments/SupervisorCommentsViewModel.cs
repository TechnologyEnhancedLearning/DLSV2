using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    public class SupervisorCommentsViewModel
    {
        public SelfAssessmentSupervisor? SelfAssessmentSupervisor { get; set; }
        public SupervisorComment? SupervisorComment { get; set; }
        public AssessmentQuestion AssessmentQuestion { get; set; }

        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(SupervisorComment?.Vocabulary);
        }
    }
}
