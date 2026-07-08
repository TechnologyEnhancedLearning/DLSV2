namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Web.Models;
    public class RoleRequirementQuestionCellsViewModel
    {
        public AssessmentQuestionModel AssessmentQuestion { get; set; }
        public int AssessmentID { get; set; }
        public int CompetencyID { get; set; }
        public int AssessmentQuestionID { get; set; }
        public string VocabularySingular { get; set; }
    }
}
