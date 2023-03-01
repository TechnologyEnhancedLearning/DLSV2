namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System.Collections.Generic;
    public class FrameworkCompetencyGroup : CompetencyGroupBase
    {
        public int Ordering { get; set; }
        public List<FrameworkCompetency> FrameworkCompetencies { get; set; } = new List<FrameworkCompetency>();
    }
}
