namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System.Collections.Generic;
    public class FrameworkCompetencyGroup
    {
        public int ID { get; set; }
        public int CompetencyGroupID { get; set; }
        public string Name { get; set; }
        public int Ordering { get; set; }
        public List<FrameworkCompetency>? FrameworkCompetencies {get; set; } = new List<FrameworkCompetency>();
    }
}
