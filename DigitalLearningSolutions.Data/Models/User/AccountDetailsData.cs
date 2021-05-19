namespace DigitalLearningSolutions.Data.Models.User
{
    public class AccountDetailsData
    {
        public AccountDetailsData(
            int? adminId,
            int? delegateId,
            string password,
            string firstName,
            string surname,
            string email,
            byte[]? profileImage
            )
        {
            AdminId = adminId;
            DelegateId = delegateId;
            Password = password;
            FirstName = firstName;
            Surname = surname;
            Email = email;
            ProfileImage = profileImage;
        }

        public int? AdminId { get; set; }
        public int? DelegateId { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public byte[]? ProfileImage { get; set; }
    }
}
