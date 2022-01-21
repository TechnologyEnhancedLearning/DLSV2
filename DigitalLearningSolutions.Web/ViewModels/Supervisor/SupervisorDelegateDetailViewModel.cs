namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public class SupervisorDelegateDetailViewModel : SupervisorDelegateDetail
    {
        public SupervisorDelegateDetailViewModel() {}

        public SupervisorDelegateDetailViewModel(SupervisorDelegateDetail supervisorDelegateDetail, int? returnPage) : base(supervisorDelegateDetail)
        {
            ReturnPage = returnPage;
        }
        public int? ReturnPage { get; set; }
    }
}
