namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class RequestSignOffViewModel
    {
        public CurrentSelfAssessment? SelfAssessment { get; set; }
        public IEnumerable<SelfAssessmentSupervisor>? Supervisors { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a supervisor")]
        public int CandidateAssessmentSupervisorId { get; set; }
        [Required]
        [Range(1, 1, ErrorMessage = "Please tick to confirm that you understand the request sign-off statement")]
        public bool StatementChecked { get; set; }
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
