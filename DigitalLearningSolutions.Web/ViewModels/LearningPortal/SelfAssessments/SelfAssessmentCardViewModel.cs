namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;

    public class SelfAssessmentCardViewModel : CurrentLearningItemViewModel
    {
        public SelfAssessmentCardViewModel(CurrentLearningItem course, ReturnPageQuery returnPageQuery) : base(
            course,
            returnPageQuery
        )
        { }
    }
}
