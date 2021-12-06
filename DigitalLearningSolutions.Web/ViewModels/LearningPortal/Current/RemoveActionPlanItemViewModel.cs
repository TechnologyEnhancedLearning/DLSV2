namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class RemoveActionPlanItemViewModel
    {
        public RemoveActionPlanItemViewModel(int learningLogItemId, string name)
        {
            LearningLogItemId = learningLogItemId;
            Name = name;
        }

        public int LearningLogItemId { get; set; }

        public string Name { get; set; }
    }
}
