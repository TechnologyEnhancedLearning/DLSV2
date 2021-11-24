namespace DigitalLearningSolutions.Data.Models.User
{
    public class EditDelegateDetailsData : AccountDetailsData
    {
        public EditDelegateDetailsData(
            int delegateId,
            string firstName,
            string surname,
            string email,
            string? alias
        ) : base(firstName, surname, email)
        {
            DelegateId = delegateId;
            Alias = alias;
        }
        
        public int DelegateId { get; set; }
        public string? Alias { get; set; }
    }
}
