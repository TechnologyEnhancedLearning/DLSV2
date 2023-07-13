
namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Delegates
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    public class SearchableDelegatesViewModel : BaseFilterableViewModel
    {
        public readonly bool CanShowDeleteDelegateButton;
        public readonly bool CanShowInactivateDelegateButton;
        public SearchableDelegatesViewModel(
           DelegateEntity delegates,
           DelegateAccount delegateAccount,
           ReturnPageQuery returnPageQuery
       )
        {
            Id = delegates.DelegateAccount.Id;
            Name = delegates.UserAccount?.FirstName + " " + delegates.UserAccount?.LastName;
            FirstName = delegates.UserAccount?.FirstName;
            LastName = delegates.UserAccount?.LastName;
            PrimaryEmail = delegates.UserAccount?.PrimaryEmail;
            UserAccountID = delegates.UserAccount.Id;
            Centre = delegates.DelegateAccount.CentreName;
            CentreEmail = delegates.UserCentreDetails?.Email;
            DelegateNumber = delegates.DelegateAccount.CandidateNumber;
            LearningHubID = delegates.UserAccount.LearningHubAuthId;
            AccountClaimed = delegates.DelegateAccount.RegistrationConfirmationHash;
            DateRegistered = delegates.DelegateAccount?.DateRegistered.ToString(Data.Helpers.DateHelper.StandardDateFormat);
            SelRegistered = delegates.DelegateAccount.SelfReg;
            IsDelegateActive = delegates.DelegateAccount.Active;
            IsCentreEmailVerified = delegates.UserAccount.EmailVerified == null ? "Unverified" : "Verified";
            CanShowInactivateDelegateButton = IsDelegateActive ;
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
        public string IsCentreEmailVerified { get; set; }
        public string DelegateNumber { get; set; }
        public int? LearningHubID { get; set; }
        public bool IsLocked { get; set; }
        public string AccountClaimed { get; set; }
        public string? DateRegistered { get; set; }
        public bool SelRegistered { get; set; }
        public bool IsDelegateActive { get; set; }
        public bool IsUserActive { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
