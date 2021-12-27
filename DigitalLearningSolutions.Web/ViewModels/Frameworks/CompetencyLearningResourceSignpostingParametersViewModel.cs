using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;
using System.Linq;
using SelfAssessmentQuestion = DigitalLearningSolutions.Data.Models.SelfAssessments.AssessmentQuestion;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyLearningResourceSignpostingParametersViewModel : BaseSignpostingViewModel
    {
        public CompetencyResourceAssessmentQuestionParameter AssessmentQuestionParameter { get; set; }
        public string Competency { get; set; }
        public string ResourceName { get; set; }
        public List<AssessmentQuestion> Questions { get; set; }
        public int? SelectedQuestionId { get; set; }
        public AssessmentQuestion SelectedQuestion
        {
            get
            {
                return Questions?.FirstOrDefault(q => q.ID == SelectedQuestionId);
            }
        }
        public SelfAssessmentQuestion MinimumQuestion { get; set; }
        public SelfAssessmentQuestion MaximumQuestion { get; set; }
        public List<LevelDescriptor> AssessmentQuestionLevelDescriptors { get; set; }
        public CompetencyLearningResourceSignpostingParametersViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId) : base(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
        {

        }
        public CompetencyLearningResourceSignpostingParametersViewModel()
        {

        }

    }
}
