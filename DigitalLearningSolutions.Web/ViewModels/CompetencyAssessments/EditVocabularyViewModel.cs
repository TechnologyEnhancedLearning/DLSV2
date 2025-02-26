using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class EditVocabularyViewModel
    {
        public EditVocabularyViewModel() { }
        public EditVocabularyViewModel(CompetencyAssessmentBase competencyAssessmentBase, bool? taskStatus)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            Vocabulary = competencyAssessmentBase.Vocabulary;
            UserRole = competencyAssessmentBase.UserRole;
            TaskStatus = taskStatus;
        }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Select a vocabulary option")]
        public string Vocabulary { get; set; }
        public int UserRole { get; set; }
        public bool? TaskStatus { get; set; }
    }
}
