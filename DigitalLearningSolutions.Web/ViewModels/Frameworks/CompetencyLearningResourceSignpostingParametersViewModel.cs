using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyLearningResourceSignpostingParametersViewModel : BaseSignpostingViewModel
    {
        public CompetencyResourceAssessmentQuestionParameter AssessmentQuestionParameter { get; set; }
        public string FrameworkCompetency { get; set; }
        public string ResourceName { get; set; }
        public List<AssessmentQuestion> Questions { get; set; }
        public AssessmentQuestion SelectedQuestion { get; set; }
        public AssessmentQuestion SelectedCompareToQuestion { get; set; }
        public int SelectedQuestionRoleRequirements { get; set; }
        public CompareAssessmentQuestionType? SelectedCompareQuestionType { get; set; }
        public List<LevelDescriptor> AssessmentQuestionLevelDescriptors { get; set; }
        public int[] SelectedLevelValues { get; set; }
        public bool CompetencyAssessmentQuestionRoleRequirements { get; set; }
        public bool TriggerValuesConfirmed { get; set; }
        public bool CompareQuestionConfirmed { get; set; }
        public CompetencyLearningResourceSignpostingParametersViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId) : base(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
        {

        }
        public CompetencyLearningResourceSignpostingParametersViewModel()
        {

        }

        public string GetLevelLabel(int value)
        {
            return AssessmentQuestionLevelDescriptors.FirstOrDefault(d => d.LevelValue == value)?.LevelLabel ?? value.ToString();
        }

    }
}
