namespace DigitalLearningSolutions.Web.Models
{
    using DigitalLearningSolutions.Data.Models.User;

    public class DelegateLoginDetails
    {
        public int Id { get; set; }
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public string? FirstName { get; set; }
        public string LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string CandidateNumber { get; set; }

        public DelegateLoginDetails() { }

        public DelegateLoginDetails(DelegateUser delegateUser)
        {
            Id = delegateUser.Id;
            CentreId = delegateUser.CentreId;
            CentreName = delegateUser.CentreName;
            FirstName = delegateUser.FirstName;
            LastName = delegateUser.LastName;
            EmailAddress = delegateUser.EmailAddress;
            CandidateNumber = delegateUser.CandidateNumber;
        }
    }
}
