namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class EditLearningLogItemCompleteByDateViewModel : EditLearningLogItemCompleteByDateFormData
    {
        public EditLearningLogItemCompleteByDateViewModel(
            int learningLogItemId,
            string name
        )
        {
            LearningLogItemId = learningLogItemId;
            Name = name;
        }

        public EditLearningLogItemCompleteByDateViewModel(
            EditLearningLogItemCompleteByDateFormData formData,
            int learningLogItemId
        ) : base(formData)
        {
            LearningLogItemId = learningLogItemId;
        }

        public int LearningLogItemId { get; set; }

        public string Name { get; set; }
    }
}
