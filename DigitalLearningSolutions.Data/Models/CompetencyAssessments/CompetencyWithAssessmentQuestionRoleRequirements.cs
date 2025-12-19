using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public class CompetencyWithAssessmentQuestionRoleRequirements
    {
        public int? CompetencyGroupID { get; set; }
        public string? GroupName { get; set; }
        public int CompetencyID { get; set; }
        public string? Competency { get; set; }
        public string? CompetencyDescription { get; set; }
        public bool? Optional { get; set; }
        public int AssessmentQuestionID { get; set; }
        public string? Question { get; set; }
        public string? InputTypeName { get; set; }
        public int? ResponseValue { get; set; }
        public string? Response { get; set; }
        public int? LevelRAG { get; set; }
    }
}
