namespace DigitalLearningSolutions.Data.Models.User
{
    public class Delegate
    {
        public Delegate(
            DelegateAccount delegateAccount,
            UserAccount userAccount,
            UserCentreDetails? userCentreDetails
        )
        {
            DelegateAccount = delegateAccount;
            UserAccount = userAccount;
            UserCentreDetails = userCentreDetails;
        }

        public DelegateAccount DelegateAccount { get; set; }
        public UserAccount UserAccount { get; set; }
        public UserCentreDetails? UserCentreDetails { get; set; }
    }
}
