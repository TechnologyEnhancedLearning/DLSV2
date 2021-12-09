namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class MarkActionPlanResourceAsCompleteViewModel : SetLearningLogItemCompletionDateFormData
    {
        public MarkActionPlanResourceAsCompleteViewModel(
            int learningLogItemId,
            string resourceName
        )
        {
            LearningLogItemId = learningLogItemId;
            ResourceName = resourceName;
        }

        public MarkActionPlanResourceAsCompleteViewModel(
            SetLearningLogItemCompletionDateFormData formData,
            int learningLogItemId
        ) : base(formData)
        {
            LearningLogItemId = learningLogItemId;
        }

        public int LearningLogItemId { get; set; }
    }
}
