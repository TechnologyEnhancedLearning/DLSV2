using System.Collections.Generic;
using DigitalLearningSolutions.Data.Models.Frameworks;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class SignpostingCardViewModel
    {
        public int? AssessmentQuestionId { get; set; }
        public int? CompetencyLearningResourceId { get; set; }
        public string Name { get; set; }
        public string AssessmentQuestion { get; set; }
        public List<LevelDescriptor> AssessmentQuestionLevelDescriptors { get; set; }
        public int AssessmentQuestionInputTypeId { get; set; }
        public int MinimumResultMatch { get; set; }
        public int MaximumResultMatch { get; set; }
        public string CompareResultTo { get; set; }
        public bool Essential { get; set; }
        public bool ParameterHasNotBeenSet { get; set; }

        public string GetLevelLabel(int value)
        {
            return AssessmentQuestionLevelDescriptors != null && AssessmentQuestionInputTypeId != 2 ?
                AssessmentQuestionLevelDescriptors[value].LevelLabel
                :value.ToString();
        }
    }
}
