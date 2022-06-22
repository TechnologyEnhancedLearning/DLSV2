namespace DigitalLearningSolutions.Data.Models.User
{
    public class DelegateEntity
    {
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

        public DelegateAccount DelegateAccount { get; }
        public UserAccount UserAccount { get; }
        public UserCentreDetails? UserCentreDetails { get; }

        public string GetEmailForCentreNotifications()
        {
            if (UserCentreDetails?.EmailVerified != null)
            {
                return UserCentreDetails?.Email ?? UserAccount.PrimaryEmail;
            }

            return UserAccount.PrimaryEmail;
        }
    }
}
