namespace DigitalLearningSolutions.Data.Models.User
{
    public class EditAccountDetailsData : AccountDetailsData
    {
        public EditAccountDetailsData(
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

        public int UserId { get; }
        public byte[]? ProfileImage { get; }
        public int JobGroupId { get; }
        public string? ProfessionalRegistrationNumber { get; }
        public bool HasBeenPromptedForPrn { get; }
    }
}
