namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System;

    public class CollaboratorNotification : CollaboratorDetail
    {
        public string InvitedByEmail { get; set; }
        public string InvitedByName { get; set; }
        public string FrameworkName { get; set; }
    }
}
