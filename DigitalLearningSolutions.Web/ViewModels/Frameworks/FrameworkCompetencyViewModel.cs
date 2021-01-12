namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public class FrameworkCompetencyViewModel
    {
        public int FrameworkId { get; set; }
        public int? FrameworkCompetencyGroupId { get; set; }
        public FrameworkCompetency FrameworkCompetency { get; set; }
    }
}
