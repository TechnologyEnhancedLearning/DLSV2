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
        public bool LevelDescriptorsAreZeroBased { get; set; }
        public int AssessmentQuestionInputTypeId { get; set; }
        public int MinimumResultMatch { get; set; }
        public int MaximumResultMatch { get; set; }
        public string CompareResultTo { get; set; }
        public bool Essential { get; set; }
        public bool ParameterHasNotBeenSet { get; set; }

        public string GetLevelLabel(int value)
        {
            int index = value - (LevelDescriptorsAreZeroBased ? 0 : 1);
            bool isNotSlider = AssessmentQuestionInputTypeId != 2;
            return AssessmentQuestionLevelDescriptors != null && isNotSlider ?
                AssessmentQuestionLevelDescriptors[index].LevelLabel
                :value.ToString();
        }
    }
}
