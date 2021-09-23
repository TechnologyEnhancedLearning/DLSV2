namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class VerificationPickSupervisorViewModel
    {
        public int SelfAssessmentId { get; set; }
        public string? Vocubulary { get; set; }
        public string? SelfAssessmentName { get; set; }
        public List<SelfAssessmentSupervisor>? Supervisors { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a supervisor")]
        public int CandidateAssessmentSupervisorId { get; set; }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(Vocubulary);
        }
    }
}
