namespace DigitalLearningSolutions.Data.Models
{
    public class PossibleEmailUpdate
    {
        public string? OldEmail { get; set; }
        public string? NewEmail { get; set; }
        public bool NewEmailIsVerified { get; set; }
        public bool IsDelegateEmailSetByAdmin { get; set; }
        public bool IsEmailUpdating => !string.Equals(OldEmail, NewEmail);
    }
}
