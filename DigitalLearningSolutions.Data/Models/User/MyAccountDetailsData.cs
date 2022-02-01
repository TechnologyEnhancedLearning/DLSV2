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
            string? professionalRegNumber,
            bool hasBeenPromptedForPrn,
            byte[]? profileImage
        ) : base(firstName, surname, email)
        {
            AdminId = adminId;
            DelegateId = delegateId;
            Password = password;
            ProfessionalRegistrationNumber = professionalRegNumber;
            HasBeenPromptedForPrn = hasBeenPromptedForPrn;
            ProfileImage = profileImage;
        }

        public MyAccountDetailsData(
            int? delegateId,
            string firstName,
            string surname,
            string email
        ) : base(firstName, surname, email)
        {
            Password = string.Empty;
            DelegateId = delegateId;
        }
        
        public int? AdminId { get; set; }
        public int? DelegateId { get; set; }
        public string Password { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
    }
}
