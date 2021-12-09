namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class RemoveActionPlanResourceViewModel
    {
        public RemoveActionPlanResourceViewModel(int learningLogItemId, string name)
        {
            LearningLogItemId = learningLogItemId;
            Name = name;
        }

        public int LearningLogItemId { get; set; }

        public string Name { get; set; }
    }
}
