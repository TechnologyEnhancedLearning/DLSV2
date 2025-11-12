using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Attributes;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ConfirmChangePrimaryFrameworkViewModel
    {
        public ConfirmChangePrimaryFrameworkViewModel()
        {}
        public ConfirmChangePrimaryFrameworkViewModel(CompetencyAssessmentBase competencyAssessmentBase, DetailFramework framework)
        {
            CompetencyAssessmentId = competencyAssessmentBase.ID;
            AssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            FrameworkName = framework.FrameworkName;
            FrameworkId = framework.ID;
            Vocabulary = competencyAssessmentBase.Vocabulary;
        }
        public int CompetencyAssessmentId { get; set; }
        public int UserRole { get; set; }
        public string? AssessmentName { get; set; }
        public string? FrameworkName { get; set; }
        public int FrameworkId { get; set; }
        public string? Vocabulary { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "You must confirm that you wish to change the primary framework")]
        public bool Confirm { get; set; }
    }
}
