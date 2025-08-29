namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SelectOptionalCompetenciesFormData
    {
        public int ID { get; set; }
        public bool? TaskStatus { get; set; }
        public int[] SelectedCompetencyIds { get; set; } = [];
    }
}
