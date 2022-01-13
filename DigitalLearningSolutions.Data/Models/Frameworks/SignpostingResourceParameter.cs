using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class SignpostingResourceParameter
    {
        public int? AssessmentQuestionId { get; set; }
        public int? CompetencyLearningResourceId { get; set; }
        public string OriginalResourceName { get; set; }
        public bool Essential { get; set; }
        public string Question { get; set; }
        public int MinResultMatch { get; set; }
        public int MaxResultMatch { get; set; }
        public string CompareResultTo { get; set; }
        public bool IsNew { get; set; }
    }
}
