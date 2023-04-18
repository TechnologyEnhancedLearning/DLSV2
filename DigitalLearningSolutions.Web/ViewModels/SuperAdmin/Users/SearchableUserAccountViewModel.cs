namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableUserAccountViewModel : BaseFilterableViewModel
    {
        public readonly bool CanShowDeactivateAdminButton;

        public SearchableUserAccountViewModel(
            UserAccountEntity user,
            ReturnPageQuery returnPageQuery
        )
        {

            Id = user.UserAccount.Id;
            Name = user.UserAccount.FirstName + " " + user.UserAccount.LastName;
            Email = user.UserAccount.PrimaryEmail;
            FirstName = user.UserAccount.FirstName;
            LastName = user.UserAccount.LastName;
            IsActive = user.UserAccount.Active;
            IsLocked = (user.UserAccount.FailedLoginCount >= AuthHelper.FailedLoginThreshold);
            JobGroupName = user.JobGroup.JobGroupName;
            IsEmailVerified = (user.UserAccount.EmailVerified != null);
            ProfessionalRegistrationNumber = user.UserAccount.ProfessionalRegistrationNumber;
            LearningHubAuthId = user.UserAccount.LearningHubAuthId;
            ReturnPageQuery = returnPageQuery;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsActive { get; set; }

        public bool IsLocked { get; set; }

        public string JobGroupName { get; set; }

        public bool IsEmailVerified { get; set; }

        public string ProfessionalRegistrationNumber { get; set; }

        public int? LearningHubAuthId { get; set; }

        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
