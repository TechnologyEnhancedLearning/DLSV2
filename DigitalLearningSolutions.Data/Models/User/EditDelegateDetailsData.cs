namespace DigitalLearningSolutions.Data.Models.User
{
    public class EditDelegateDetailsData : AccountDetailsData
    {
        public EditDelegateDetailsData(
            int delegateId,
            string firstName,
            string surname,
            string email,
            string? professionalRegNumber,
            bool hasBeenPromptedForPrn
        ) : base(firstName, surname, email)
        {
            DelegateId = delegateId;
            ProfessionalRegistrationNumber = professionalRegNumber;
            HasBeenPromptedForPrn = hasBeenPromptedForPrn;
        }

        public int DelegateId { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
    }
}
