namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public class CompetencyGroupCardViewModel
    {
        public FrameworkCompetencyGroup FrameworkCompetencyGroup { get; set; }
        public bool CanModify { get; set; }
    }
}
