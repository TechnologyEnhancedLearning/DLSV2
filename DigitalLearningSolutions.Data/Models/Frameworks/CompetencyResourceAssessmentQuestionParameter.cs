using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class CompetencyResourceAssessmentQuestionParameter
    {
        public int? AssessmentQuestionId { get; set; }
        public int ResourceRefId { get; set; }
        public int AssessmentQuestionInputTypeId { get; set; }
        public int AssessmentQuestionMinValue { get; set; }
        public int AssessmentQuestionMaxValue { get; set; }
        public int CompetencyLearningResourceId { get; set; }
        public int MinResultMatch { get; set; }
        public int MaxResultMatch { get; set; }
        public bool Essential { get; set; }
        public int? RelevanceAssessmentQuestionId { get; set; }
        public bool CompareToRoleRequirements { get; set; }
        public string OriginalResourceName { get; set; }
        public string OriginalResourceType { get; set; }
        public decimal OriginalRating { get; set; }
        public string Question { get; set; }
        public string CompareResultTo { get; set; }
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
