namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class RemoveActionPlanResourceViewModel
    {
        public RemoveActionPlanResourceViewModel(int learningLogItemId, string name, bool absentInLearningHub, bool resourceSourcedFromFallbackData)
        {
            LearningLogItemId = learningLogItemId;
            Name = name;
            AbsentInLearningHub = absentInLearningHub;
            ResourceSourcedFromFallbackData = resourceSourcedFromFallbackData;
        }

        public int LearningLogItemId { get; set; }

        public string Name { get; set; }

        public bool AbsentInLearningHub { get; set; }

        public bool ResourceSourcedFromFallbackData { get; set; }
    }
}
