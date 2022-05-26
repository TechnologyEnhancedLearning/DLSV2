namespace DigitalLearningSolutions.Data.Models.User
{
    public class MyAccountDetailsData : AccountDetailsData
    {
        public MyAccountDetailsData(
            int? adminId,
            int? delegateId,
            string firstName,
            string surname,
            string email,
            int? jobGroupId,
            string? professionalRegNumber,
            bool hasBeenPromptedForPrn,
            byte[]? profileImage
        ) : base(firstName, surname, email)
        {
            AdminId = adminId;
            DelegateId = delegateId;
            JobGroupId = jobGroupId;
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
            DelegateId = delegateId;
        }

        public int? AdminId { get; set; }
        public int? DelegateId { get; set; }
        public byte[]? ProfileImage { get; set; }
        public int? JobGroupId { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
    }
}
