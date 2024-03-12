namespace DigitalLearningSolutions.Data.Models
{
    public class NotificationPreference
    {
        public int NotificationId { get; set; }
        public string NotificationName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Accepted { get; set; }
    }
}
