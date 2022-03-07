namespace DigitalLearningSolutions.Data.Models.User
{
    public class EditDelegateDetailsData : AccountDetailsData
    {
        public EditDelegateDetailsData(
            int delegateId,
            string firstName,
            string surname,
            string email,
            string? alias,
            string? professionalRegNumber,
            bool hasBeenPromptedForPrn
        ) : base(firstName, surname, email)
        {
            DelegateId = delegateId;
            Alias = alias;
            ProfessionalRegistrationNumber = professionalRegNumber;
            HasBeenPromptedForPrn = hasBeenPromptedForPrn;
        }
        
        public int DelegateId { get; set; }
        public string? Alias { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
    }
}
