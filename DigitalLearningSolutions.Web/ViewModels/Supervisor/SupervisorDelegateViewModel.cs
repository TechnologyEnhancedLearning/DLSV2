using DigitalLearningSolutions.Web.ViewModels.Supervisor;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Data.Models.Supervisor;

namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    public class SupervisorDelegateViewModel
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int CandidateAssessmentCount { get; set; }
        public string DelegateEmail { get; set; }

        [BooleanMustBeTrue(ErrorMessage = "Confirm you wish to remove this staff member")]
        public bool ConfirmedRemove { get; set; }

        public SupervisorDelegateViewModel(SupervisorDelegateDetail detail)
        {
            Id = detail.ID;
            FirstName = detail.FirstName;
            LastName = detail.LastName;
            DelegateEmail = detail.DelegateEmail;
            CandidateAssessmentCount = detail.CandidateAssessmentCount;
        }
        public SupervisorDelegateViewModel()
        {

        }
    }
}
