namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using DigitalLearningSolutions.Data.Models.User;

    public class MyAccountViewModel
    {
        public MyAccountViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            string? centreName)
        {
            FirstName = adminUser?.FirstName ?? delegateUser?.FirstName;
            Surname = adminUser?.LastName ?? delegateUser?.LastName;
            User = adminUser?.EmailAddress ?? delegateUser?.EmailAddress;
            ProfilePicture = adminUser?.ProfileImage ?? delegateUser?.ProfileImage;
            Centre = centreName;
            DelegateNumber = delegateUser?.CandidateNumber;
        }

        public string? Centre { get; set; }

        public string? User { get; set; }

        public string? DelegateNumber { get; set; }

        public string? FirstName { get; set; }

        public string? Surname { get; set; }

        public byte[]? ProfilePicture { get; set; }
    }
}
