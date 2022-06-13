namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public class SupervisorDelegateDetailViewModel : BaseSearchableItem
    {
        public SupervisorDelegateDetailViewModel() { }

        public SupervisorDelegateDetailViewModel(SupervisorDelegateDetail supervisorDelegateDetail, ReturnPageQuery returnPageQuery , bool isUserSupervisor = false)
        {
            SupervisorDelegateDetail = supervisorDelegateDetail;
            ReturnPageQuery = returnPageQuery;
            IsUserSupervisor = isUserSupervisor;
        }

        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }
        public bool IsUserSupervisor { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? $"{SupervisorDelegateDetail.FirstName} {SupervisorDelegateDetail.LastName} {SupervisorDelegateDetail.DelegateEmail}";
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
