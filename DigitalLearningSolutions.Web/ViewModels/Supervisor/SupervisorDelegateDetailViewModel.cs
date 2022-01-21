namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public class SupervisorDelegateDetailViewModel : SupervisorDelegateDetail
    {
        public SupervisorDelegateDetailViewModel() { }

        public SupervisorDelegateDetailViewModel(SupervisorDelegateDetail supervisorDelegateDetail, int? page) : base(
            supervisorDelegateDetail
        )
        {
            Page = page;
        }

        public int? Page { get; set; }
    }
}
