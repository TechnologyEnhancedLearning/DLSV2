using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class CollaboratorDetail : Collaborator
    {
        public string? Email { get; set; }
        public string? Forename { get; set; }
        public string? Surname { get; set; }
        public string? FrameworkRole { get; set;  }
    }
}
