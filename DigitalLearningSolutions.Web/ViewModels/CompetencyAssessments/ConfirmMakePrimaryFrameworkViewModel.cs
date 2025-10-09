using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Data.Models.Frameworks;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ConfirmMakePrimaryFrameworkViewModel
    {
        public ConfirmMakePrimaryFrameworkViewModel()
        {}
        public ConfirmMakePrimaryFrameworkViewModel(CompetencyAssessmentBase competencyAssessmentBase, DetailFramework framework)
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
        [Required(ErrorMessage = "You need to confirm that you want to make this the primary framework")]
        public bool? Confirm { get; set; }
        public bool DescriptionStatus { get; set; }
        public bool ProviderandCategoryStatus { get; set; }
        public bool VocabularyStatus { get; set; }
        public bool WorkingGroupStatus { get; set; }
        public bool AllframeworkCompetenciesStatus { get; set; }
    }
}
