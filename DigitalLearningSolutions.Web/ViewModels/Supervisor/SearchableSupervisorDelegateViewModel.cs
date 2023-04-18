namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.Supervisor;
    public class SearchableSupervisorDelegateViewModel
    {
        public SearchableSupervisorDelegateViewModel(SupervisorDelegateDetail supervisorDelegateDetail)
        {
            SupervisorDelegateDetail = supervisorDelegateDetail;
        }

        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
    }
}
