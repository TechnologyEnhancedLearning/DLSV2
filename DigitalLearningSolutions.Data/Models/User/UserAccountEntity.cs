namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class UserAccountEntity : BaseSearchableItem
    {

        // This type needs a parameterless constructor when it replaces the type T in GenericSearchHelper.SearchItems
        public UserAccountEntity()
        {
            UserAccount = new UserAccount();
            JobGroup = new JobGroup();
        }

        public UserAccountEntity(
            UserAccount userAccount,
            JobGroup jobGroup
        )
        {
            UserAccount = userAccount;
            JobGroup = jobGroup;
        }

        public UserAccount UserAccount { get; }

        public JobGroup JobGroup { get; }

        public override string SearchableName {get; set;}

    }
}
