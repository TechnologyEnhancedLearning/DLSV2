namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class VerificationPickSupervisorViewModel
    {
        public CurrentSelfAssessment? SelfAssessment { get; set; }
        public List<SelfAssessmentSupervisor>? Supervisors { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a supervisor")]
        public int CandidateAssessmentSupervisorId { get; set; }
        public string VocabPlural()
        {
            if (SelfAssessment != null)
            {
                return FrameworkVocabularyHelper.VocabularyPlural(SelfAssessment.Vocabulary);
            }
            else
            {
                return "Capabilities";
            }
        }
    }
}
