namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public class CompetencyCardViewModel
    {
        public FrameworkCompetency FrameworkCompetency { get; set; }
        public bool CanModify { get; set; }
        public int FrameworkCompetencyGroupId { get; set; }
    }
}
