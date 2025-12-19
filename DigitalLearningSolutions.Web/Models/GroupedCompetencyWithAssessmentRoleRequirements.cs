using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Models
{
    public class GroupedCompetencyWithAssessmentRoleRequirements
    {
        public int CompetencyGroupID { get; set; } = 0;
        public string GroupName { get; set; } = "Ungrouped";
        public List<CompetencyModel> Competencies { get; set; } = new();
    }

    public class CompetencyModel
    {
        public int CompetencyID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? Optional { get; set; }
        public List<AssessmentQuestionModel> Questions { get; set; } = new();
    }

    public class AssessmentQuestionModel
    {
        public int AssessmentQuestionID { get; set; }
        public string? Question { get; set; }
        public string? InputTypeName { get; set; }
        public List<ResponseModel> Responses { get; set; } = new();
    }
    public class ResponseModel
    {
        public int? ResponseValue { get; set; }
        public string? ResponseLabel { get; set; }
        public int? LevelRAG { get; set; }
    }

}

