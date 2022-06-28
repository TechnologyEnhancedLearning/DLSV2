namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Helpers;

    public class AdminEntity
    {
        public AdminEntity(
            AdminAccount adminAccount,
            UserAccount userAccount,
            UserCentreDetails? userCentreDetails
        )
        {
            AdminAccount = adminAccount;
            UserAccount = userAccount;
            UserCentreDetails = userCentreDetails;
        }

        public AdminAccount AdminAccount { get; }
        public UserAccount UserAccount { get; }
        public UserCentreDetails? UserCentreDetails { get; }

        public string EmailForCentreNotifications => CentreEmailHelper.GetEmailForCentreNotifications(
            UserAccount.PrimaryEmail,
            UserCentreDetails?.Email
        );
    }
}
