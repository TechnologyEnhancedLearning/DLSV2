namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class RemoveActionPlanResourceViewModel
    {
        public RemoveActionPlanResourceViewModel(int learningLogItemId, string name, bool absentInLearningHub)
        {
            LearningLogItemId = learningLogItemId;
            Name = name;
            AbsentInLearningHub = absentInLearningHub;
        }

        public int LearningLogItemId { get; set; }

        public string Name { get; set; }

        public bool AbsentInLearningHub { get; set; }
    }
}
