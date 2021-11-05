namespace DigitalLearningSolutions.Data.Models.User
{
    public class MyAccountDetailsData : AccountDetailsData
    {
        public MyAccountDetailsData(
            int? adminId,
            int? delegateId,
            string password,
            string firstName,
            string surname,
            string email,
            byte[]? profileImage
        ) : base(firstName, surname, email)
        {
            AdminId = adminId;
            DelegateId = delegateId;
            Password = password;

            ProfileImage = profileImage;
        }

        public int? AdminId { get; set; }
        public int? DelegateId { get; set; }
        public string Password { get; set; }
        public byte[]? ProfileImage { get; set; }
    }
}
