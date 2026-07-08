namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class CompetencyAssessmentPreviewViewModel
    {
        public CompetencyAssessmentPreviewViewModel()
        { }
        public CompetencyAssessmentPreviewViewModel(int competencyAssessmentId,string competencyAssessmentName, int centreId, string centreName)
        {
            CompetencyAssessmentId = competencyAssessmentId;
            CompetencyAssessmentName = competencyAssessmentName;
            CentreId = centreId;
            CentreName = centreName;
        }
        public int CompetencyAssessmentId { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public int CentreId { get; set; }
        public string CentreName { get; set; } = string.Empty;
    }
}
