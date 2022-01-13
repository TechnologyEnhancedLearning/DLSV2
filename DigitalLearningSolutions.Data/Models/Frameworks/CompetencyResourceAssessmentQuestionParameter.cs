using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class CompetencyResourceAssessmentQuestionParameter
    {
        public int? AssessmentQuestionId { get; set; }
        public int CompetencyLearningResourceId { get; set; }
        public int AssessmentQuestionID { get; set; }
        public int MinResultMatch { get; set; }
        public int MaxResultMatch { get; set; }
        public bool Essential { get; set; }
        public int? RelevanceAssessmentQuestionID { get; set; }
        public bool CompareToRoleRequirements { get; set; }
        public string Competency { get; set; }

        public AssessmentQuestion AssessmentQuestion { get; set; }
        public AssessmentQuestion RelevanceAssessmentQuestion { get; set; }
        public bool IsNew { get; set; }

        public CompetencyResourceAssessmentQuestionParameter(bool isNew)
        {
            this.IsNew = isNew;
        }
        public CompetencyResourceAssessmentQuestionParameter()
        {

        }
    }
}
