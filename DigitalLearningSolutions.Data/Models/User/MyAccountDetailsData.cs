namespace DigitalLearningSolutions.Data.Models.User
{
    public class MyAccountDetailsData : AccountDetailsData
    {
        public MyAccountDetailsData(
            int userId,
            string firstName,
            string surname,
            string email,
            int jobGroupId,
            string? professionalRegNumber,
            bool hasBeenPromptedForPrn,
            byte[]? profileImage
        ) : base(firstName, surname, email)
        {
            UserId = userId;
            JobGroupId = jobGroupId;
            ProfessionalRegistrationNumber = professionalRegNumber;
            HasBeenPromptedForPrn = hasBeenPromptedForPrn;
            ProfileImage = profileImage;
        }

        public int UserId { get; set; }
        public byte[]? ProfileImage { get; set; }
        public int JobGroupId { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
    }
}
