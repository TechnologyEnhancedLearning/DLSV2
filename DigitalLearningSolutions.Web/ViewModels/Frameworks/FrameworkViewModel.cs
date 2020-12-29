namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System.Collections.Generic;
    using System.Linq;

    public class FrameworkViewModel
    {
        public DetailFramework DetailFramework { get; set; }
        public IEnumerable<CollaboratorDetail> Collaborators {get; set;}
        public List<FrameworkCompetencyGroup> FrameworkCompetencyGroups { get; set; }
        public IEnumerable<FrameworkCompetency> FrameworkCompetencies { get; set; }
    }
}
