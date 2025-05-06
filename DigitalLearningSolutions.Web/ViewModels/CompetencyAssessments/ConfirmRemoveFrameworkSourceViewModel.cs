using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Attributes;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ConfirmRemoveFrameworkSourceViewModel
    {
        public ConfirmRemoveFrameworkSourceViewModel() { }
        public ConfirmRemoveFrameworkSourceViewModel(CompetencyAssessmentBase competencyAssessmentBase, DetailFramework framework, int competencyCount)
        {
            CompetencyAssessmentId = competencyAssessmentBase.ID;
            CompetencyCount = competencyCount;
            AssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            FrameworkName = framework.FrameworkName;
            FrameworkId = framework.ID;
            Vocabulary = competencyAssessmentBase.Vocabulary;
        }
        public int CompetencyAssessmentId { get; set; }
        public string? AssessmentName { get; set; }
        public string? FrameworkName { get; set; }
        public int FrameworkId { get; set; }
        public int CompetencyCount { get; set; }
        public string? Vocabulary { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "You must confirm that you wish to remove this framework")]
        public bool Confirm { get; set; }

    }
}
