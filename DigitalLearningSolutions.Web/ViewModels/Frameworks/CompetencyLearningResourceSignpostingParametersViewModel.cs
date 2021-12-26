using DigitalLearningSolutions.Data.Models.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyLearningResourceSignpostingParametersViewModel : BaseSignpostingViewModel
    {
        public string Competency { get; set; }
        public string ResourceName { get; set; }
        public List<AssessmentQuestion> Questions { get; set; }
        public CompetencyResourceAssessmentQuestionParameter AssessmentQuestionParameter { get; set; }
        public int? SelectedQuestionId { get; set; }
        public CompetencyLearningResourceSignpostingParametersViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId) : base(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
        {

        }
        public CompetencyLearningResourceSignpostingParametersViewModel()
        {

        }

    }
}
