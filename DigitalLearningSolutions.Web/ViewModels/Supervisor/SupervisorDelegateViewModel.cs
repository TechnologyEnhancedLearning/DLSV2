namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Attributes;

    public class SupervisorDelegateViewModel
    {
        public SupervisorDelegateViewModel(SupervisorDelegateDetail detail, ReturnPageQuery returnPageQuery)
        {
            Id = detail.ID;
            FirstName = detail.FirstName;
            LastName = detail.LastName;
            DelegateEmail = detail.DelegateEmail;
            CandidateAssessmentCount = detail.CandidateAssessmentCount;
            ReturnPageQuery = returnPageQuery;
        }
        public SupervisorDelegateViewModel() { }
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int CandidateAssessmentCount { get; set; }
        public string DelegateEmail { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }

        [BooleanMustBeTrue(ErrorMessage = "Please tick the checkbox to confirm you wish to perform this action")]
        public bool ActionConfirmed { get; set; }
    }
}
