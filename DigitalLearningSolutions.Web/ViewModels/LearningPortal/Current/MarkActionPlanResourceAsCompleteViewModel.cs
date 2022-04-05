namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class MarkActionPlanResourceAsCompleteViewModel : MarkActionPlanResourceAsCompleteFormData
    {
        public MarkActionPlanResourceAsCompleteViewModel(
            int learningLogItemId,
            bool absentInLearningHub,
            string resourceName,
            bool apiIsAccessible,
            ReturnPageQuery returnPageQuery
        )
        {
            LearningLogItemId = learningLogItemId;
            AbsentInLearningHub = absentInLearningHub;
            ResourceName = resourceName;
            ApiIsAccessible = apiIsAccessible;
            ReturnPageQuery = returnPageQuery;
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
