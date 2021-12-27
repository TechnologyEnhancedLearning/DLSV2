using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;
using SelfAssessmentQuestion = DigitalLearningSolutions.Data.Models.SelfAssessments.AssessmentQuestion;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class SignpostingParametersSetStatusViewModel : BaseSignpostingViewModel
    {
        public SelfAssessmentQuestion MinimumQuestion { get; set; }
        public SelfAssessmentQuestion MaximumQuestion { get; set; }
        public AssessmentQuestion SelectedQuestion { get; set; }
        public List<LevelDescriptor> AssessmentQuestionLevelDescriptors { get; set; }

        public SignpostingParametersSetStatusViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId): base(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
        {

        }
        public SignpostingParametersSetStatusViewModel()
        {

        }
    }
}
