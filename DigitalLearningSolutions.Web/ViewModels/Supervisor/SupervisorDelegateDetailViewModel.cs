namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public class SupervisorDelegateDetailViewModel : BaseSearchableItem
    {
        public SupervisorDelegateDetailViewModel() { }

        public SupervisorDelegateDetailViewModel(SupervisorDelegateDetail supervisorDelegateDetail, int? page)
        {
            SupervisorDelegateDetail = supervisorDelegateDetail;
            Page = page;
        }

        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public int? Page { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? $"{SupervisorDelegateDetail.FirstName} {SupervisorDelegateDetail.LastName} {SupervisorDelegateDetail.DelegateEmail}";
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
