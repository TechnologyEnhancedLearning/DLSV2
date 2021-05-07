namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;

    public class CollaboratorsViewModel
    {
        public int FrameworkId { get; set; }
        public string? FrameworkName { get; set; }
        public IEnumerable<CollaboratorDetail>? Collaborators { get; set; }
        public int AdminID { get; set; }
    }
}
