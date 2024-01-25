
namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Delegates
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SuperAdmin;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    public class SearchableDelegatesViewModel : BaseFilterableViewModel
    {
        public readonly bool CanShowDeleteDelegateButton;
        public readonly bool CanShowInactivateDelegateButton;
        public SearchableDelegatesViewModel(
           SuperAdminDelegateAccount delegates,
           ReturnPageQuery returnPageQuery
       )
        {
            Id = delegates.Id;
            Name = delegates.FirstName + " " + delegates.LastName;
            FirstName = delegates?.FirstName;
            LastName = delegates?.LastName;
            PrimaryEmail = delegates.EmailAddress;
            UserAccountID = delegates.UserId;
            Centre = delegates.CentreName;
            CentreEmail = delegates.CentreEmail;
            DelegateNumber = delegates.CandidateNumber;
            LearningHubID = delegates.LearningHubAuthId;
            AccountClaimed = delegates.RegistrationConfirmationHash;
            DateRegistered = delegates.DateRegistered?.ToString(Data.Helpers.DateHelper.StandardDateFormat);
            SelRegistered = delegates.SelfReg;
            IsDelegateActive = delegates.Active;
            IsCentreEmailVerified = delegates.CentreEmailVerified == null ? false : true;
            CanShowInactivateDelegateButton = IsDelegateActive;
            IsUserActive = delegates.UserActive;
            IsApproved = delegates.Approved;
            IsClaimed = delegates.RegistrationConfirmationHash == null ? true : false;
            ReturnPageQuery = returnPageQuery;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryEmail { get; set; }
        public int UserAccountID { get; set; }
        public string Centre { get; set; }
        public string CentreEmail { get; set; }
        public bool IsCentreEmailVerified { get; set; }
        public string DelegateNumber { get; set; }
        public int? LearningHubID { get; set; }
        public bool IsLocked { get; set; }
        public string AccountClaimed { get; set; }
        public string? DateRegistered { get; set; }
        public bool SelRegistered { get; set; }
        public bool IsDelegateActive { get; set; }
        public bool IsUserActive { get; set; }
        public bool IsApproved { get; set; }
        public bool IsClaimed { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
