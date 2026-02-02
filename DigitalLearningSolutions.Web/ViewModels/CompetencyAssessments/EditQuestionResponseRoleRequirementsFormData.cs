using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class EditQuestionResponseRoleRequirementsFormData
    {
        /// <summary>
        /// Key = ResponseValue
        /// Value = RAG level (1,2,3) or null
        /// </summary>
        [Required]
        public Dictionary<int, int?> ResponseRoleRequirements { get; set; }
            = new Dictionary<int, int?>();
        public bool ApplyToAll { get; set; }
        public int Id { get; set; }
        public int CompetencyId { get; set; }
        public int AssessmentQuestionId { get; set; }

    }
}
