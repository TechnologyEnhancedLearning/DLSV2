namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class RemoveActionPlanResourceViewModel
    {
        public RemoveActionPlanResourceViewModel(
            int learningLogItemId,
            string name,
            bool absentInLearningHub,
            bool apiIsAccessible
        )
        {
            LearningLogItemId = learningLogItemId;
            Name = name;
            AbsentInLearningHub = absentInLearningHub;
            ApiIsAccessible = apiIsAccessible;
        }

        public int LearningLogItemId { get; set; }

        public string Name { get; set; }

        public bool AbsentInLearningHub { get; set; }

        public bool ApiIsAccessible { get; set; }

        public ReturnPageQuery? ReturnPageQuery { get; set; }
    }
}
