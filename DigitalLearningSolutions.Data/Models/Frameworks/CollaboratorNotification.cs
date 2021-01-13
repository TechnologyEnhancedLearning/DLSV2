namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class CollaboratorNotification : CollaboratorDetail
    {
        public string InvitedByEmail { get; set; }
        public string InvitedByName { get; set; }
        public string FrameworkName { get; set; }
    }
}
