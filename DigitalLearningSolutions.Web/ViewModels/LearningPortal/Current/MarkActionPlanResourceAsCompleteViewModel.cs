namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class MarkActionPlanResourceAsCompleteViewModel : MarkActionPlanResourceAsCompleteFormData
    {
        public MarkActionPlanResourceAsCompleteViewModel(
            int learningLogItemId,
            bool absentInLearningHub,
            string resourceName,
            bool resourceSourcedFromFallbackData
        )
        {
            LearningLogItemId = learningLogItemId;
            AbsentInLearningHub = absentInLearningHub;
            ResourceName = resourceName;
            ResourceSourcedFromFallbackData = resourceSourcedFromFallbackData;
        }

        public MarkActionPlanResourceAsCompleteViewModel(
            MarkActionPlanResourceAsCompleteFormData formData,
            int learningLogItemId
        ) : base(formData)
        {
            LearningLogItemId = learningLogItemId;
        }

        public int LearningLogItemId { get; set; }
    }
}
