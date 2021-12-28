using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections.Generic;
using System.Linq;

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
        public List<LevelDescriptor> AssessmentQuestionLevelDescriptors { get; set; }
        public int[] SelectedLevelValues { get; set; }
        public CompetencyLearningResourceSignpostingParametersViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId) : base(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
        {

        }
        public CompetencyLearningResourceSignpostingParametersViewModel()
        {

        }

    }
}
