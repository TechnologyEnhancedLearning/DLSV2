namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class CollaboratorDetail : Collaborator
    {
        public string? UserEmail { get; set; }
        public bool? UserActive { get; set; }
        public string? FrameworkRole { get; set;  }
    }
}
