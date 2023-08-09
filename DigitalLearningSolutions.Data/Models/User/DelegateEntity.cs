namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class DelegateEntity: BaseSearchableItem
    { // This type needs a parameterless constructor when it replaces the type T in GenericSearchHelper.SearchItems
        public DelegateEntity()
        {
            DelegateAccount = new DelegateAccount();
            UserAccount = new UserAccount();
        }

        public DelegateEntity(
            DelegateAccount delegateAccount,
            UserAccount userAccount,
            UserCentreDetails? userCentreDetails
        )
        {
            DelegateAccount = delegateAccount;
            UserAccount = userAccount;
            UserCentreDetails = userCentreDetails;
        }

        public DelegateEntity(
            DelegateAccount delegateAccount,
            UserAccount userAccount,
            UserCentreDetails? userCentreDetails,
            int? adminId
        )
        {
            DelegateAccount = delegateAccount;
            UserAccount = userAccount;
            UserCentreDetails = userCentreDetails;
            AdminId = adminId;
        }

        public DelegateAccount DelegateAccount { get; }
        public UserAccount UserAccount { get; }
        public UserCentreDetails? UserCentreDetails { get; }
        public int? AdminId { get; }
        public string EmailForCentreNotifications => CentreEmailHelper.GetEmailForCentreNotifications(
            UserAccount.PrimaryEmail,
            UserCentreDetails?.Email
        );
        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ??
                   NameQueryHelper.GetSortableFullName(UserAccount.FirstName, UserAccount.LastName);
            set => SearchableNameOverrideForFuzzySharp = value;
        }
        public RegistrationFieldAnswers GetRegistrationFieldAnswers()
        {
            return new RegistrationFieldAnswers(
                DelegateAccount.CentreId,
                UserAccount.JobGroupId,
                DelegateAccount.Answer1,
                DelegateAccount.Answer2,
                DelegateAccount.Answer3,
                DelegateAccount.Answer4,
                DelegateAccount.Answer5,
                DelegateAccount.Answer6
            );
        }
    }
}
