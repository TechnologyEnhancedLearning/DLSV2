namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class CompetencyAssessmentPreviewViewModel
    {
        public CompetencyAssessmentPreviewViewModel()
        { }
        public CompetencyAssessmentPreviewViewModel(int competencyAssessmentId, int centreId, string centreName)
        {
            CompetencyAssessmentId = competencyAssessmentId;
            CentreId = centreId;
            CentreName = centreName;
        }
        public int CompetencyAssessmentId { get; set; }
        public int CentreId { get; set; }
        public string CentreName { get; set; } = string.Empty;
    }
}
