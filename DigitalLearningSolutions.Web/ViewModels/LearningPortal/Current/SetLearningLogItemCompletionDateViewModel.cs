namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class SetLearningLogItemCompletionDateViewModel : SetLearningLogItemCompletionDateFormData
    {
        public SetLearningLogItemCompletionDateViewModel(
            int learningLogItemId,
            string resourceName
        )
        {
            LearningLogItemId = learningLogItemId;
            ResourceName = resourceName;
        }

        public SetLearningLogItemCompletionDateViewModel(
            SetLearningLogItemCompletionDateFormData formData,
            int learningLogItemId
        ) : base(formData)
        {
            LearningLogItemId = learningLogItemId;
        }

        public int LearningLogItemId { get; set; }

        public string ResourceName { get; set; }
    }
}
