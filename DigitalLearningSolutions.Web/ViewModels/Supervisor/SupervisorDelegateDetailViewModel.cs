namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public class SupervisorDelegateDetailViewModel : BaseSearchableItem
    {
        public SupervisorDelegateDetailViewModel() { }

        public SupervisorDelegateDetailViewModel(SupervisorDelegateDetail supervisorDelegateDetail, ReturnPageQuery returnPageQuery, bool isUserSupervisor = false, int? loggedInUserId = 0)
        {
            SupervisorDelegateDetail = supervisorDelegateDetail;
            ReturnPageQuery = returnPageQuery;
            IsUserSupervisor = isUserSupervisor;
            LoggedInUserId = loggedInUserId;
        }

        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }
        public bool IsUserSupervisor { get; set; }
        public int? LoggedInUserId { get; set; }

        public string LoggedInUserStyle()
        {
            if (SupervisorDelegateDetail.DelegateUserID == LoggedInUserId)
            {
                return "loggedinuser";
            }
            return "";
        }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? $"{SupervisorDelegateDetail.FirstName} {SupervisorDelegateDetail.LastName} {SupervisorDelegateDetail.DelegateEmail}";
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
