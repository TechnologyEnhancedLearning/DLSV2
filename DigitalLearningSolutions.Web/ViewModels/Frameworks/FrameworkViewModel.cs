namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System.Collections.Generic;
    public class FrameworkViewModel
    {
        public DetailFramework detailFramework { get; set; }
        public IEnumerable<CollaboratorDetail> collaborators {get; set;}
        public IEnumerable<FrameworkCompetencyGroup> frameworkCompetencyGroups { get; set; }
        public IEnumerable<FrameworkCompetency> frameworkCompetencies { get; set; }
    }
}
