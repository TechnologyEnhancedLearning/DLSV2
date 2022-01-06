namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Enums;

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
            YesNoSelectionEnum yesNoSelection,
            byte[]? profileImage
        ) : base(firstName, surname, email)
        {
            AdminId = adminId;
            DelegateId = delegateId;
            Password = password;
            ProfessionRegistrationNumber = professionalRegNumber;
            HasBeenPromptedForPrn = yesNoSelection != YesNoSelectionEnum.None;

            ProfileImage = profileImage;
        }

        public int? AdminId { get; set; }
        public int? DelegateId { get; set; }
        public string Password { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? ProfessionRegistrationNumber { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
    }
}
