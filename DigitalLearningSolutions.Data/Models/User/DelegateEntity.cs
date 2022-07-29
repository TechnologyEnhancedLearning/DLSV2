namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Helpers;

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
