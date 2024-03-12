namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System;

    public class CollaboratorNotification : CollaboratorDetail
    {
        public string InvitedByEmail { get; set; } = string.Empty;
        public string InvitedByName { get; set; } = string.Empty;
        public string FrameworkName { get; set; } = string.Empty;
    }
}
